using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace GBHO_Common.Controllers
{
    /// <summary>
    /// Provides methods to help read and write to CSV files
    /// </summary>
    public class CSVHelper
    {
        private const char delimiter = ','; // should we let them specified this?
        private const char quote = '"';

        /// <summary>
        /// creates a valid CSV row
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public static string CreateRow(params string[] items)
        {
            StringBuilder row = new StringBuilder();
            foreach (string item in items)
            {
                row.Append(delimiter).Append(quote).Append(item.Replace("\"", "\"\"")).Append(quote);
            }

            return row.Remove(0, 1).Append(Environment.NewLine).ToString();
        }

        /// <summary>
        /// Extracts information from the data object provided and writes it to the specified output stream
        /// </summary>
        /// <param name="data">the data object</param>
        /// <param name="writeHeader">Whether to include a header row using information provided in the data object</param>
        /// <param name="output">the stream to write the data directly to</param>
        public static void CreateFromData(IDataReader data, bool writeHeader, TextWriter output)
        {
            if (data != null && !data.IsClosed)
            {
                // write the header if we have to or haven't yet
                if (writeHeader)
                {
                    for (int i = 0; i < data.FieldCount; i++)
                    {
                        output.Write(quote);
                        output.Write(data.GetName(i).Replace("\"", "\"\""));
                        output.Write(quote);
                        output.Write(delimiter);
                    }
                    output.WriteLine();
                }

                // write the values
                while (data.Read())
                {
                    for (int i = 0; i < data.FieldCount; i++)
                    {
                        object val = data.GetValue(i);
                        if (val != DBNull.Value)
                        {
                            output.Write(quote);
                            output.Write(val.ToString().Replace("\"", "\"\""));
                            output.Write(quote);
                        }
                        output.Write(delimiter);
                    }
                    output.WriteLine();
                }
            }
        }

        /// <summary>
        /// Extracts comma delimited quoted or non-quoted items from a row
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public static List<string> ParseRow(string row)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Extracts one row of comma delimited quoted or non-quoted items from the cursor. An item can span multiple lines.
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static List<string> ParseNextRow(StreamReader reader)
        {
            /* Parsing notes / assumptions
             * Fields are comma delimited
             * fields containing new lines are enclosed in double quotes (")
             * Double quotes within encased double quotes are double quoted ("")
             */
            bool startNewValue = true;
            List<string> values = new List<string>();
            StringBuilder value = null;
            while (!reader.EndOfStream)
            {
                string glue = Environment.NewLine;
                string line = reader.ReadLine();
                string[] parts = line.Split(',');
                for (int i = 0; i < parts.Length; i++)
                {
                    string part = parts[i];
                    if (startNewValue)
                    {
                        value = new StringBuilder(part);

                        // starting a quoted field value
                        if (value.Length > 0 && value[0] == quote)
                        {
                            value.Remove(0, 1);

                            // and ending a quoted field all in the one go
                            if (ContainsEndingQuote(value.ToString()))
                            {
                                value.Remove(value.Length - 1, 1);
                            }
                            else
                            {
                                // add the split comma or new line back in
                                value.Append((i == parts.Length - 1) ? Environment.NewLine : ",");
                                startNewValue = false;
                            }
                        }
                    }
                    else
                    {
                        value.Append(part);

                        // ending a quoted field (is always quoted if we end up here)
                        if (ContainsEndingQuote(part))
                        {
                            value.Remove(value.Length - 1, 1);
                            startNewValue = true;
                        }
                        else
                            // add the split comma or new line back in
                            value.Append((i == parts.Length - 1) ? Environment.NewLine : ",");
                    }

                    // store it if we've collected everything for this value
                    if (value != null && startNewValue)
                    {
                        value.Replace("\"\"", "\""); // make sure we unescape double quotes
                        values.Add(value.ToString());
                    }
                }

                // at the end of the row
                if (startNewValue)
                    break;
            }

            return values;
        }

        private static bool ContainsEndingQuote(string value)
        {
            // need to be careful because the field may have a double
            // quote before the comma eg "an ""example"", and "another one"""
            int count = 0;
            if (value.Length > 0)
            {
                for (int i = value.Length - 1; i >= 0; i--)
                {
                    if (value[i] == quote)
                        count++;
                    else
                        break;
                }
            }

            // generally, if it's an odd number, then it's ended, else, they're just double quoted
            return (count % 2 == 1);
        }
    }
}
