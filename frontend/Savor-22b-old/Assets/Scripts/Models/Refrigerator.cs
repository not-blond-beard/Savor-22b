using System;

[Serializable]
public class Refrigerator
{
    public Guid stateId;
    public string grade;
    public int hp;
    public int attack;
    public int defense;
    public int speed;

    public int? ingredientId;
    public int? recipeId;
}