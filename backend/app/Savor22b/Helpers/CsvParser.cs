namespace Savor22b.Helpers;

using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Immutable;

public class CsvParser<T> where T : class, new()
{
    public ImmutableList<T> ParseCsv(string filePath)
    {
        ImmutableList<T> items = ImmutableList<T>.Empty;

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
                    items = items.Add(item);
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

                if (fieldName.Contains("List") && fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(ImmutableList<>))
                {
                    Type listType = underlyingType.GetGenericArguments()[0];
                    string[] stringValues = fields[i].Split(';');

                    object list;

                    if (fieldType == typeof(ImmutableList<int>))
                    {
                        list = ImmutableList<int>.Empty;
                    }
                    else if (fieldType == typeof(ImmutableList<string>))
                    {
                        list = ImmutableList<string>.Empty;
                    }
                    else
                    {
                        continue;
                    }

                    foreach (var value in stringValues)
                    {
                        if (!string.IsNullOrEmpty(value))
                        {
                            object typedValue = Convert.ChangeType(value, listType);
                            list = list.GetType().GetMethod("Add").Invoke(list, new[] { typedValue });
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
