namespace Savor22b.Helpers;

using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.IO;

public class CsvParser<T> where T : class, new()
{
    public List<T> ParseCsv(string filePath)
    {
        List<T> items = new List<T>();

        using (TextFieldParser parser = new TextFieldParser(filePath))
        {
            parser.TextFieldType = FieldType.Delimited;
            parser.SetDelimiters(",");

            parser.ReadLine();

            while (!parser.EndOfData)
            {
                string[] fields = parser.ReadFields();

                T item = MapFieldsToObject(fields);
                if (item != null)
                {
                    items.Add(item);
                }
            }
        }

        return items;
    }

    private T MapFieldsToObject(string[] fields)
    {
        Type type = typeof(T);
        T item = new T();

        if (fields.Length > 0)
        {
            for (int i = 0; i < fields.Length; i++)
            {
                string fieldName = type.GetProperties()[i].Name;
                Type fieldType = type.GetProperties()[i].PropertyType;
                Type underlyingType = Nullable.GetUnderlyingType(fieldType) ?? fieldType;

                if (fieldName.Contains("List") && fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    Type listType = underlyingType.GetGenericArguments()[0];
                    string[] stringValues = fields[i].Split(';');
                    var list = Activator.CreateInstance(fieldType) as System.Collections.IList;

                    foreach (var value in stringValues)
                    {
                        if (!string.IsNullOrEmpty(value))
                        {
                            list.Add(Convert.ChangeType(value, listType));
                        }
                    }

                    type.GetProperty(fieldName).SetValue(item, list);
                }
                else if (!string.IsNullOrEmpty(fields[i]))
                {
                    object fieldValue = Convert.ChangeType(fields[i], underlyingType);
                    type.GetProperty(fieldName).SetValue(item, fieldValue);
                }
            }

            return item;
        }

        return null;
    }
}
