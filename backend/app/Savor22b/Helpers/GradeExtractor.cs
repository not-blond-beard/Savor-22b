using System;
using Libplanet.Action;

namespace Savor22b.Helpers;

public class GradeExtractor
{
    private IRandom randomHelper;
    private double probability;
    public GradeExtractor(IRandom random, double probability)
    {
        this.randomHelper = random;
        this.probability = probability;
    }
    public int ExtractGrade(string minGrade, string maxGrade)
    {
        if (IsLuckyGrade())
        {
            return GetGradeValue(maxGrade);
        }
        else
        {
            int minGradeValue = GetGradeValue(minGrade);
            int maxGradeValue = GetGradeValue(maxGrade);

            return randomHelper.Next(minGradeValue, maxGradeValue + 1);
        }
    }

    private bool IsLuckyGrade()
    {
        double normalizedValue = (double)this.randomHelper.Next() / int.MaxValue;
        return normalizedValue <= this.probability;
    }

    static public int GetGradeValue(string grade)
    {
        switch (grade.ToUpper())
        {
            case "SS":
                return 2;
            case "S":
                return 1;
            case "A":
                return 0;
            case "B":
                return -1;
            case "C":
                return -2;
            case "D":
                return -3;
            default:
                throw new ArgumentException("Invalid grade: " + grade);
        }
    }

    static public string GetGrade(int gradeValue)
    {
        switch (gradeValue)
        {
            case 2:
                return "SS";
            case 1:
                return "S";
            case 0:
                return "A";
            case -1:
                return "B";
            case -2:
                return "C";
            case -3:
                return "D";
            default:
                throw new ArgumentException("Invalid grade value: " + gradeValue);
        }
    }
}
