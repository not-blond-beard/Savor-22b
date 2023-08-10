namespace Savor22b.Action;

using System.Collections.Immutable;
using Bencodex.Types;
using Libplanet.Action;
using Libplanet.State;
using Savor22b.Helpers;
using Savor22b.Model;
using Savor22b.States;
using Libplanet.Headless.Extensions;
using Savor22b.Constants;
using Libplanet;
using Savor22b.Action.Exceptions;

[ActionType(nameof(PlaceUserHouseAction))]
public class PlaceUserHouseAction : SVRAction
{
    public int VillageID;
    public int TargetX;
    public int TargetY;


    public PlaceUserHouseAction()
    {
    }

    public PlaceUserHouseAction(int villageID, int targetX, int targetY)
    {
        VillageID = villageID;
        TargetX = targetX;
        TargetY = targetY;
    }

    protected override IImmutableDictionary<string, IValue> PlainValueInternal =>
        new Dictionary<string, IValue>()
        {
            [nameof(VillageID)] = VillageID.Serialize(),
            [nameof(TargetX)] = TargetX.Serialize(),
            [nameof(TargetY)] = TargetY.Serialize(),
        }.ToImmutableDictionary();

    protected override void LoadPlainValueInternal(
        IImmutableDictionary<string, IValue> plainValue)
    {
        VillageID = plainValue[nameof(VillageID)].ToInteger();
        TargetX = plainValue[nameof(TargetX)].ToInteger();
        TargetY = plainValue[nameof(TargetY)].ToInteger();
    }

    private Village? getVillageData(int villageId)
    {
        CsvParser<Village> csvParser = new CsvParser<Village>();

        var csvPath = Paths.GetCSVDataPath("villages.csv");
        var village = csvParser.ParseCsv(csvPath);

        return village.Find(e => e.Id == villageId);
    }

    private bool isAbleToPlaceHouse(Village village, int targetX, int targetY)
    {
        int halfWidth = (village.Width - 1) / 2;
        int halfHeight = (village.Height - 1) / 2;

        // halfWidth는 중심점을 기준으로 좌우 몇칸 설치가 가능한 지를 나타냄. 그러니 절댓값 targetX가 halfWidth보다 크면 설치 불가

        if (Math.Abs(targetX) > halfWidth)
        {
            return false;
        }

        if (Math.Abs(targetY) > halfHeight)
        {
            return false;
        }

        return true;
    }

    public override IAccountStateDelta Execute(IActionContext ctx)
    {
        IAccountStateDelta states = ctx.PreviousStates;

        var village = getVillageData(VillageID);

        if (village == null)
        {
            throw new InvalidVillageException("Invalid village ID");
        }

        if (!isAbleToPlaceHouse(village, TargetX, TargetY))
        {
            throw new InvalidVillageException("Invalid target position");
        }

        GlobalUserHouseState globalUserHouseState =
            states.GetState(Addresses.UserHouseDataAddress) is Bencodex.Types.Dictionary stateEncoded
                ? new GlobalUserHouseState(stateEncoded)
                : new GlobalUserHouseState();

        string userHouseKey = globalUserHouseState.CreateKey(VillageID, TargetX, TargetY);

        if (globalUserHouseState.UserHouse.ContainsKey(userHouseKey))
        {
            throw new HouseAlreadyPlacedException("House already placed");
        }

        RootState rootState = states.GetState(ctx.Signer) is Bencodex.Types.Dictionary rootStateEncoded
            ? new RootState(rootStateEncoded)
            : new RootState();

        string prevUserHouseKey = rootState.VillageState?.HouseState != null
            ? globalUserHouseState.CreateKey(
                rootState.VillageState.HouseState.VillageID,
                rootState.VillageState.HouseState.PositionX,
                rootState.VillageState.HouseState.PositionY
            )
            : string.Empty;

        if (rootState.VillageState == null)
        {
            rootState.SetVillageState(new VillageState(new HouseState(
                VillageID,
                TargetX,
                TargetY,
                new HouseInnerState()
            )));
        }
        else
        {
            rootState.VillageState.SetHouseState(new HouseState(
                VillageID,
                TargetX,
                TargetY,
                new HouseInnerState()
            ));
        }

        if (prevUserHouseKey != string.Empty)
        {
            globalUserHouseState.UserHouse.Remove(prevUserHouseKey);
        }

        globalUserHouseState.SetUserHouse(userHouseKey, ctx.Signer);

        states = states.SetState(Addresses.UserHouseDataAddress, globalUserHouseState.Serialize());
        states = states.SetState(ctx.Signer, rootState.Serialize());

        return states;
    }
}
