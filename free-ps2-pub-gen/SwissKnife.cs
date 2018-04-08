using Microsoft.Win32;
using free_ps2_pub_gen.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Media;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace free_ps2_pub_gen {
    /// <summary>
    /// Enumuration for the CursorPosition.
    /// </summary>
    public enum CursorPosition {
        /// <summary>
        /// No Cursor Position.
        /// </summary>
        None,
        /// <summary>
        /// Left Cursor Position.
        /// </summary>
        Left,
        /// <summary>
        /// Right Cursor Position.
        /// </summary>
        Right
    }

    /// <summary>
    /// Enumuration for the Button Position.
    /// </summary>
    public enum ButtonPosition {
        /// <summary>
        /// No Button Position.
        /// </summary>
        None,
        /// <summary>
        /// Left Button Position.
        /// </summary>
        Left,
        /// <summary>
        /// Centered Button Position.
        /// </summary>
        Center,
        /// <summary>
        /// Right Button Position.
        /// </summary>
        Right
    }

    /// <summary>
    /// Enumuration for Yes No Cancel button.
    /// </summary>
    public enum Buttons {
        /// <summary>
        /// OK Button.
        /// </summary>
        Ok,
        /// <summary>
        /// Yes Button.
        /// </summary>
        Yes,
        /// <summary>
        /// Yes and No Buttons.
        /// </summary>
        YesNo,
        /// <summary>
        /// Yes, No and Chancel Buttons.
        /// </summary>
        YesNoCancel,
        /// <summary>
        /// Ok and No Buttons.
        /// </summary>
        OkNo,
        /// <summary>
        /// Ok, Bo and Chancel Buttons.
        /// </summary>
        OkNoCancel
    }

    /// <summary>
    /// File Extension.
    /// </summary>
    public static class FileEx {
        /// <summary>
        /// Remove All Empty Lines from a file.
        /// </summary>
        /// <param name="file">The File from where to remove all empty lines.</param>
        public static void RemoveEmptyLines(string file) {
            string[] lines = File.ReadAllLines(file);
            List<string> cleaned = new List<string>();

            foreach (string line in lines) if (!string.IsNullOrEmpty(line)) cleaned.Add(line);

            File.Delete(file);
            File.Create(file).Close();
            File.WriteAllLines(file, cleaned.ToArray());
        }
    }

    /// <summary>
    /// Some StringExtansion.
    /// </summary>
    static class StringExtension {
        /// <summary>
        /// StringComparison to ignore Cases for our modded string extansions.
        /// </summary>
        private static StringComparison ignoreCase = StringComparison.CurrentCultureIgnoreCase;

        /// <summary>
        /// A string that represents a valid Hex Value.
        /// </summary>
        private static string hex = "0123456789ABCDEFabcdef";

        /// <summary>
        /// Checks if a string do contain a specific string including StringCoparison option.
        /// </summary>
        /// <param name="source">The source string to check.</param>
        /// <param name="toCheck">The string that shall be looked for.</param>
        /// <param name="comparison">String Comparison options.</param>
        /// <returns>True if the string to look for was found., else false.</returns>
        public static bool Contains(this string source, string toCheck, StringComparison comparison) { return source.IndexOf(toCheck, comparison) >= 0; }

        /// <summary>
        /// Checks if a string do contain a specific string including StringCoparison option. (Auto ignoreCase)
        /// </summary>
        /// <param name="source">The source string to check.</param>
        /// <param name="toCheck">The string that shall be looked for.</param>
        /// <returns>True if the string to look for was found., else false.</returns>
        public static bool Contain(this string source, string toCheck) { return source.IndexOf(toCheck, ignoreCase) >= 0; }

        /// <summary>
        /// Reverse a Hexstring BigEndian wise.
        /// </summary>
        /// <param name="source">The soruce Hex String to use.</param>
        /// <returns>The Big Endian Swapped Hex String.</returns>
        public static string EndianSwapp(this string source) {
            string reversed = string.Empty;
            for (int i = source.Length; i > 0; i -= 2) {
                if (i < 2) reversed += source.Substring(i - i, 1);
                else reversed += source.Substring(i - 2, 2);
            }
            return reversed;
        }

        /// <summary>
        /// Gets recursively all Files from the given path matching the search Pattern, including Sub Directories.
        /// </summary>
        /// <param name="source">The path to look up.</param>
        /// <param name="searchPattern">A specifc character combination to search for.</param>
        /// <returns>A string Array with all files including sub directories, pending the searchPattern, listed.</returns>
        public static string[] GetFilesNDirsRecursive(this string source, string searchPattern) {
            List<string> result = new List<string>();
            foreach (string file in Directory.GetFiles(source, searchPattern)) { result.Add(file); }
            foreach (string subDir in Directory.GetDirectories(source)) {
                result.Add(subDir);
                result.AddRange(subDir.GetFilesNDirsRecursive(searchPattern));
            }
            return result.ToArray();
        }

        /// <summary>
        /// Gets recursively all Files from the given path, including Sub Directories.
        /// </summary>
        /// <param name="source">The path to look up.</param>
        /// <returns>A string Array with all files and sub directories listed.</returns>
        public static string[] GetFilesNDirsRecursive(this string source) {
            List<string> result = new List<string>();
            foreach (string file in Directory.GetFiles(source)) { result.Add(file); }
            foreach (string subDir in Directory.GetDirectories(source)) {
                result.Add(subDir);
                result.AddRange(subDir.GetFilesNDirsRecursive());
            }
            return result.ToArray();
        }

        /// <summary>
        /// Formats a string and replaces all the arguments from the params object array with the placeholders within the string.
        /// </summary>
        /// <param name="toFormat">The string to format.</param>
        /// <param name="format">A params object array containing all the arguments to replace.</param>
        /// <returns>The formated string.</returns>
        public static string FormatString(this string toFormat, params object[] format) {
            for (int i = 0; i < format.Length; i++) {                                                                                         // We loop over the amount of arguments defined. This ensure that we don't replace more then arguments present.
                if (toFormat.Contains("{" + i.ToString() + "}")) toFormat = toFormat.Replace("{" + i.ToString() + "}", format[i].ToString()); // And if there is a replace trigger '{0}' | '{1}' | etc., we replace it with the argument from the array.
                else break;                                                                                                                   // Else, if there are more Replace arguments then replace trigger, we break the loop instead.
            }
            return toFormat;                                                                                                                  // Return the formated string now.
        }

        /// <summary>
        /// Trims the path out of a name. Name can be last folder in string or file.
        /// </summary>
        /// <param name="source">The path string to trim.</param>
        /// <returns>The name without any back slashes or path in it.</returns>
        public static string GetName(this string source) {
            string[] splitted = source.Split('\\');
            return splitted[splitted.Length - 1];
        }

        /// <summary>
        /// Trim the name out of a path. Name can be last folder in string or file.
        /// </summary>
        /// <param name="source">The path string to trim.</param>
        /// <returns>The path without the last folder or the file in the string.</returns>
        public static string GetPath(this string source) {
            string[] splitted = source.Split('\\');
            return source.Replace(splitted[splitted.Length - 1], "");
        }

        /// <summary>
        /// Checks if a string is of type file or folder.
        /// </summary>
        /// <param name="source">The source string to check.</param>
        /// <returns>True if the string to check is a folder, else false.</returns>
        public static bool IsFolder(this string source) { return ((File.GetAttributes(source) & FileAttributes.Directory) == FileAttributes.Directory); }

        /// <summary>
        /// Check if a input string is a valid Hex Value.
        /// </summary>
        /// <param name="value">The value to check for.</param>
        /// <returns>True if the value contains only Hex values, else False.</returns>
        public static bool IsHex(this string value) {
            for (int i = 0; i < value.Length; i++) if (!hex.Contains(value[i].ToString())) return false;     // Loop trough the whole string and check every single digit if it is not a hex value. If so return false.
            return true;
        }

        /// <summary>
        /// XReg Replace some pattern from a source string with a given string.
        /// </summary>
        /// <param name="source">The source string to use.</param>
        /// <param name="searchPattern">The pattern to search for.</param>
        /// <param name="replaceWith">The string to replace with.</param>
        /// <returns>The Replaced string, if any found.</returns>
        public static string XReplace(this string source, string searchPattern, string replaceWith) { return Regex.Replace(source, searchPattern, replaceWith); }

        /// <summary>
        /// XReg Replace some pattern from a source string with a given string with Regex Options.
        /// </summary>
        /// <param name="source">The source string to use.</param>
        /// <param name="searchPattern">The pattern to search for.</param>
        /// <param name="replaceWith">The string to replace with.</param>
        /// <param name="options">Define some Regex options.</param>
        /// <returns>The Replaced string, if any found.</returns>
        public static string XReplace(this string source, string searchPattern, string replaceWith, RegexOptions options) { return Regex.Replace(source, searchPattern, replaceWith, options); }
    }

    /// <summary>
    /// Array Extension.
    /// </summary>
    public static class ArrayExtension {
        /// <summary>
        /// Internal used for indexer resolving.
        /// </summary>
        private static int a;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        public static T[] Resize<T>(this T[] source, int num) {
            Array.Resize(ref source, num);
            return source;
        }

        /// <summary>
        /// Clear a Array.
        /// </summary>
        /// <typeparam name="T">The Type of the array to use.</typeparam>
        /// <param name="source">The source Array.</param>
        public static void Clear<T>(this T[] source) { T[] clear = new T[source.Length]; source = clear; }

        /// <summary>
        /// Clear a Array of Arrays.
        /// </summary>
        /// <typeparam name="T">The Type of the array to use.</typeparam>
        /// <param name="source">The source Array.</param>
        public static void Clear<T>(this T[][] source) { T[][] clear = new T[source.Length][]; source = clear; }

        /// <summary>
        /// Checks a Array for existens of a value.
        /// </summary>
        /// <typeparam name="T">The Type of the array to use and the value to add.</typeparam>
        /// <param name="source">The source Array.</param>
        /// <param name="value">The value to check for existens.</param>
        /// <returns>True if the source Array contains the value to check for, else false.</returns>
        public static bool Contains<T>(this T[] source, T value) {
            if (source == null) throw new FormatException("Null Refernce", new Exception("The Array to check for a value existens is not Initialized."));
            foreach (T check in source) if (check.Equals(value)) return true;
            return false;
        }

        /// <summary>
        /// Checks a Array for existens of a other Array.
        /// </summary>
        /// <typeparam name="T">The Type of the array to use and the value to add.</typeparam>
        /// <param name="source">The source Array.</param>
        /// <param name="range">The Array to check for existens.</param>
        /// <returns>True if the source Array contains the Array to check for, else false.</returns>
        public static bool Contains<T>(this T[] source, T[] range) {
            if (source == null) throw new FormatException("Null Refernce", new Exception("The Array to check for a value existens is not Initialized."));
            for (int i = 0; i < source.Length; i++) {
                if (source[i].Equals(range[0])) {
                    if ((source.Length - i) >= range.Length) {
                        int match = 1;
                        for (int j = 1; j < range.Length; j++) {
                            if (source[i + j].Equals(range[j])) match++;
                            else { i += j; break; }
                        }
                        if (match == range.Length) return true;
                    } else break;
                }
            }
            return false;
        }

        /// <summary>
        /// Combine one or more arrays with each other.
        /// </summary>
        /// <typeparam name="T">The Type of the array to use and the value to add.</typeparam>
        /// <param name="source">The source array to which we shall combine to it.</param>
        /// <param name="arrays">The arrays which shall be combine with each other.</param>
        public static T[] Combine<T>(this T[] source, params T[][] arrays) {
            arrays.AddOnTop(source);
            T[] newArray = new T[arrays.Sum(a => a.Length)];

            int offset = 0;
            foreach (Array array in arrays) {
                Buffer.BlockCopy(array, 0, newArray, offset, array.Length);
                offset += array.Length;
            }
            return newArray;
        }

        /// <summary>
        /// Combine one or more arrays with each other on top of the source array.
        /// </summary>
        /// <typeparam name="T">The Type of the array to use and the value to add.</typeparam>
        /// <param name="source">The source array to which we shall combine to it.</param>
        /// <param name="arrays">The arrays which shall be combine with each other.</param>
        public static T[] CombineOnTop<T>(this T[] source, params T[][] arrays) {
            arrays.Add(source);
            T[] newArray = new T[arrays.Sum(a => a.Length)];

            int offset = 0;
            foreach (Array array in arrays) {
                Buffer.BlockCopy(array, 0, newArray, offset, array.Length);
                offset += array.Length;
            }
            return newArray;
        }

        /// <summary>
        /// Compares a Array against a other one.
        /// </summary>
        /// <typeparam name="T">The Type of the array to use and the value to add.</typeparam>
        /// <param name="source">The source Array.</param>
        /// <param name="range">The Array to check for.</param>
        /// <param name="sameLength">Determine if both arrays shall be of same length.</param>
        /// <returns>True if the source array contains the array to check for (optional length test), else false.</returns>
        public static bool Equals<T>(this T[] source, T[] range, [Optional] bool sameLength) {
            if (source == null) throw new FormatException("Null Refernce", new Exception("The Array to check for a value existens is not Initialized."));
            if (sameLength) if (source.Length != range.Length) return false;
            for (int i = 0; i < range.Length; i++) { if (!source[i].Equals(range[i])) return false; }
            return true;
        }

        /// <summary>
        /// Gets the index of a value within a array.
        /// </summary>
        /// <typeparam name="T">The Type of the array to use and the value to search for.</typeparam>
        /// <param name="source">The source Array.</param>
        /// <param name="value">The value to check for existens.</param>
        /// <returns>The index of the value within the array if found, else -1.</returns>
        public static int IndexOf<T>(this T[] source, T value) {
            if (source == null) throw new FormatException("Null Refernce", new Exception("The Array to check for a value existens is not Initialized."));
            for (int i = 0; i < source.Length; i++) { if (source[i].Equals(value)) return i; }
            return -1;
        }

        /// <summary>
        /// Gets the start index of a array within a array.
        /// </summary>
        /// <typeparam name="T">The Type of the array to use and the value to add.</typeparam>
        /// <param name="source">The source Array.</param>
        /// <param name="toCheck">The Array to check for the start index.</param>
        /// <returns>The start index of the array to check within the source array if found, else -1.</returns>
        public static int IndexOf<T>(this T[] source, T[] toCheck) {
            if (source == null) throw new FormatException("Null Refernce", new Exception("The Array to check for a value existens is not Initialized."));
            for (int i = 0; i < source.Length; i++) {
                if (source[i].Equals(toCheck[0])) {
                    if ((source.Length - i) >= toCheck.Length) {
                        int match = 1;
                        for (int j = 1; j < toCheck.Length; j++) {
                            if (source[i + j].Equals(toCheck[j])) match++;
                            else { i += j; break; }
                        }
                        if (match == toCheck.Length) return i;
                    } else break;
                }
            }
            return -1;
        }

        /// <summary>
        /// Gets the indexer of a array within a array of arrays.
        /// </summary>
        /// <typeparam name="T">The Type of the array to use and the value to add.</typeparam>
        /// <param name="source">The source Array.</param>
        /// <param name="toCheck">The Array to check for the start index.</param>
        /// <returns>The start index of the array to check within the source array if found, else -1.</returns>
        public static int IndexOf<T>(this T[][] source, T[] toCheck) {
            if (source == null) throw new FormatException("Null Refernce", new Exception("The Array to check for a value existens is not Initialized."));
            for (int i = 0; i < source.Length; i++) { if (source[i].Equals(toCheck)) return i; }
            return -1;
        }

        /// <summary>
        /// Gets the index of the last matching value within a array.
        /// </summary>
        /// <typeparam name="T">The Type of the array to use and the value to search for.</typeparam>
        /// <param name="source">The source Array.</param>
        /// <param name="value">The value to check for existens.</param>
        /// <returns>The index of the value within the array if found, else -1.</returns>
        public static int IndexOfLast<T>(this T[] source, T value) {
            if (source == null) throw new FormatException("Null Refernce", new Exception("The Array to check for a value existens is not Initialized."));
            for (int i = source.Length - 1; i > -1; i--) { if (source[i].Equals(value)) return i; }
            return -1;
        }

        /// <summary>
        /// Gets the index of all matching values within a array.
        /// </summary>
        /// <typeparam name="T">The Type of the array to use and the value to search for.</typeparam>
        /// <param name="source">The source Array.</param>
        /// <param name="value">The value to check for existens.</param>
        /// <returns>The index of the value within the array if found, else -1.</returns>
        public static int[] IndexOfAll<T>(this T[] source, T value) {
            if (source == null) throw new FormatException("Null Refernce", new Exception("The Array to check for a value existens is not Initialized."));
            int[] found = new int[0];
            for (int i = 0; i < source.Length; i++) if (source[i].Equals(value)) found.Add(i);
            return found;
        }

        /// <summary>
        /// Add a value to a Array.
        /// </summary>
        /// <typeparam name="T">The Type of the array to use and the value to add.</typeparam>
        /// <param name="source">The source Array.</param>
        /// <param name="value">The value to add.</param>
        /// <returns>The new Array with the added value.</returns>
        public static T[] Add<T>(this T[] source, T value) {
            T[] newArray = new T[source.Length + 1];
            Array.Copy(source, newArray, source.Length);
            newArray[newArray.Length - 1] = value;
            return newArray;
        }

        /// <summary>
        /// Add a Range of values to a Array.
        /// </summary>
        /// <typeparam name="T">The Type of the array to use and the value to add.</typeparam>
        /// <param name="source">The source Array.</param>
        /// <param name="range">The Array to add.</param>
        /// <returns>The new Array with the added value.</returns>
        public static T[] Add<T>(this T[] source, T[] range) { return source.Combine(range); }

        /// <summary>
        /// Add a Range of values to a Array.
        /// </summary>
        /// <typeparam name="T">The Type of the array to use and the value to add.</typeparam>
        /// <param name="source">The source Array.</param>
        /// <param name="range">The Array to add.</param>
        /// <returns>The new Array with the added value.</returns>
        public static T[][] Add<T>(this T[][] source, T[] range) {
            T[][] newArray = new T[source.Length + 1][];
            Array.Copy(source, newArray, source.Length);
            newArray[source.Length] = range;
            return newArray;
        }

        /// <summary>
        /// Add a value to a array but only if not already existent.
        /// </summary>
        /// <typeparam name="T">The Type of the array to use and the value to add.</typeparam>
        /// <param name="source">The source Array.</param>
        /// <param name="value">The value to add.</param>
        /// <returns>The new Array with the added value.</returns>
        public static T[] AddNew<T>(this T[] source, T value) {
            if (source.Contains(value)) return source;
            T[] newArray = new T[source.Length + 1];
            Array.Copy(source, newArray, source.Length);
            newArray[newArray.Length - 1] = value;
            return newArray;
        }

        /// <summary>
        /// Add a Range of values to a Array but only if not already existent.
        /// </summary>
        /// <typeparam name="T">The Type of the array to use and the value to add.</typeparam>
        /// <param name="source">The source Array.</param>
        /// <param name="range">The Array to add.</param>
        /// <returns>The new Array with the added value.</returns>
        public static T[] AddNew<T>(this T[] source, T[] range) {
            if (source.Contains(range)) return source;
            return source.Combine(range);
        }

        /// <summary>
        /// Add a Range of values to a Array but only if not already existent.
        /// </summary>
        /// <typeparam name="T">The Type of the array to use and the value to add.</typeparam>
        /// <param name="source">The source Array.</param>
        /// <param name="range">The Array to add.</param>
        /// <returns>The new Array with the added value.</returns>
        public static T[][] AddNew<T>(this T[][] source, T[] range) {
            if (source.Contains(range)) return source;
            T[][] newArray = new T[source.Length + 1][];
            Array.Copy(source, newArray, source.Length);
            newArray[source.Length] = range;
            return newArray;
        }

        /// <summary>
        /// Add a value on Top of a Array.
        /// </summary>
        /// <typeparam name="T">The Type of the array to use and the value to add.</typeparam>
        /// <param name="source">The source Array.</param>
        /// <param name="value">The value to add.</param>
        /// <returns>The new Array with the added value.</returns>
        public static T[] AddOnTop<T>(this T[] source, T value) {
            T[] newArray = new T[source.Length + 1];
            newArray[0] = value;
            Array.Copy(source, 0, newArray, 1, source.Length);
            return newArray;
        }

        /// <summary>
        /// Add a range of values on top of a Array.
        /// </summary>
        /// <typeparam name="T">The Type of the array to use and the value to add.</typeparam>
        /// <param name="source">The source Array.</param>
        /// <param name="range">The Array to add.</param>
        /// <returns>The new Array with the added value.</returns>
        public static T[] AddOnTop<T>(this T[] source, T[] range) { return source.CombineOnTop(range); }

        /// <summary>
        /// Add a range of values on top of a Array of Arrays.
        /// </summary>
        /// <typeparam name="T">The Type of the array to use and the value to add.</typeparam>
        /// <param name="source">The source Array of Arrays.</param>
        /// <param name="range">The Array to add.</param>
        /// <returns>The new Array with the added value.</returns>
        public static T[][] AddOnTop<T>(this T[][] source, T[] range) {
            T[][] newArray = new T[source.Length + 1][];
            newArray[0] = range;
            Array.Copy(source, 0, newArray, 1, source.Length);
            return newArray;
        }

        /// <summary>
        /// Add a value on Top of a Array but only if not already existent.
        /// </summary>
        /// <typeparam name="T">The Type of the array to use and the value to add.</typeparam>
        /// <param name="source">The source Array.</param>
        /// <param name="value">The value to add.</param>
        /// <returns>The new Array with the added value.</returns>
        public static T[] AddNewOnTop<T>(this T[] source, T value) {
            if (source.Contains(value)) return source;
            T[] newArray = new T[source.Length + 1];
            newArray[0] = value;
            Array.Copy(source, 0, newArray, 1, source.Length);
            return newArray;
        }

        /// <summary>
        /// Add a range of values on Top of a Array but only if not already existent.
        /// </summary>
        /// <typeparam name="T">The Type of the array to use and the value to add.</typeparam>
        /// <param name="source">The source Array.</param>
        /// <param name="range">The range of values to add.</param>
        /// 
        public static T[] AddNewOnTop<T>(this T[] source, T[] range) {
            if (source.Contains(range)) return source;
            return source.CombineOnTop(range);
        }

        /// <summary>
        /// Add a range of values on Top of a Array of Arrays but only if not already existent.
        /// </summary>
        /// <typeparam name="T">The Type of the array to use and the value to add.</typeparam>
        /// <param name="source">The source Array.</param>
        /// <param name="range">The range of values to add.</param>v
        /// <returns>The new Array with the added value.</returns>
        public static T[][] AddNewOnTop<T>(this T[][] source, T[] range) {
            if (source.Contains(range)) return source;
            T[][] newArray = new T[source.Length + 1][];
            newArray[0] = range;
            Array.Copy(source, 0, newArray, 1, source.Length);
            return newArray;
        }

        /// <summary>
        /// Insert a value into a Array.
        /// </summary>
        /// <typeparam name="T">The Type of the array to use and the value to add.</typeparam>
        /// <param name="source">The source Array.</param>
        /// <param name="index">The index on which we shall insert.</param>
        /// <param name="value">The value to add.</param>
        /// <returns>The new Array with the added value.</returns>
        public static T[] Insert<T>(this T[] source, int index, T value) {
            if (index == -1 || index + 1 > source.Length) return source;
            else if (index == 0) return source.AddOnTop(value);
            else if (index + 1 == source.Length) return source.Add(value);
            else {
                T[] newArray = new T[source.Length + 1];
                Array.Copy(source, newArray, index);
                newArray[index] = value;
                Array.Copy(source, index + 1, newArray, index + 1, (source.Length - (index + 1)));
                return newArray;
            }
        }

        /// <summary>
        /// Insert a range of values into a Array.
        /// </summary>
        /// <typeparam name="T">The Type of the array to use and the value to add.</typeparam>
        /// <param name="source">The source Array.</param>
        /// <param name="index">The index on which we shall insert.</param>
        /// <param name="range">The range to add.</param>
        /// <returns>The new Array with the added value.</returns>
        public static T[] Insert<T>(this T[] source, int index, T[] range) {
            if (index == -1 || index + 1 > source.Length) return source;
            else if (index == 0) return source.AddOnTop(range);
            else if (index + 1 == source.Length) return source.Add(range);
            else {
                T[] newArray = new T[source.Length + range.Length];
                Array.Copy(source, newArray, index);
                Array.Copy(range, 0, newArray, index, range.Length);
                Array.Copy(source, index, newArray, index + range.Length, source.Length - index);
                return newArray;
            }
        }

        /// <summary>
        /// Insert a range of values into a Array of Arrays.
        /// </summary>
        /// <typeparam name="T">The Type of the array to use and the value to add.</typeparam>
        /// <param name="source">The source Array.</param>
        /// <param name="index">The index on which we shall insert.</param>
        /// <param name="range">The range to add.</param>
        /// <returns>The new Array with the added value.</returns>
        public static T[][] Insert<T>(this T[][] source, int index, T[] range) {
            if (index == -1 || index + 1 > source.Length) return source;
            else if (index == 0) return source.AddOnTop(range);
            else if (index + 1 == source.Length) return source.Add(range);
            else {
                T[][] newArray = new T[source.Length + 1][];
                Array.Copy(source, newArray, index);
                newArray[index] = range;
                Array.Copy(source, index, newArray, index + 1, source.Length - index);
                return newArray;
            }
        }

        /// <summary>
        /// Insert a value into a Array.
        /// </summary>
        /// <typeparam name="T">The Type of the array to use and the value to add.</typeparam>
        /// <param name="source">The source Array.</param>
        /// <param name="index">The value on which place we shall insert.</param>
        /// <param name="value">The value to add.</param>
        /// <returns>The new Array with the added value.</returns>
        public static T[] Insert<T>(this T[] source, T index, T value) {
            if ((a = source.IndexOf(index)) != -1) return source.Insert(a, value);
            return source;
        }

        /// <summary>
        /// Insert a range of values into a Array.
        /// </summary>
        /// <typeparam name="T">The Type of the array to use and the value to add.</typeparam>
        /// <param name="source">The source Array.</param>
        /// <param name="index">The value on which place we shall insert.</param>
        /// <param name="range">The values to add.</param>
        /// <returns>The new Array with the added value.</returns>
        public static T[] Insert<T>(this T[] source, T index, T[] range) {
            if ((a = source.IndexOf(index)) != -1) return source.Insert(a, range);
            return source;
        }

        /// <summary>
        /// Insert a range of values into a Array of arrays.
        /// </summary>
        /// <typeparam name="T">The Type of the array to use and the value to add.</typeparam>
        /// <param name="source">The source Array.</param>
        /// <param name="index">The value on which place we shall insert.</param>
        /// <param name="range">The values to add.</param>
        /// <returns>The new Array with the added value.</returns>
        public static T[][] Insert<T>(this T[][] source, T[] index, T[] range) {
            if ((a = source.IndexOf(index)) != -1) return source.Insert(a, range);
            return source;
        }

        /// <summary>
        /// Replace a value within a Array.
        /// </summary>
        /// <typeparam name="T">The Type of the array to use and the value to add.</typeparam>
        /// <param name="source">The source Array.</param>
        /// <param name="toChange">The value to replace.</param>
        /// <param name="value">The value to add.</param>
        public static void Replace<T>(this T[] source, T toChange, T value) { if ((a = source.IndexOf(toChange)) != -1) source[a] = value; }

        /// <summary>
        /// Replace a range of values within a Array.
        /// </summary>
        /// <typeparam name="T">The Type of the array to use and the value to add.</typeparam>
        /// <param name="source">The source Array.</param>
        /// <param name="toChange">The values to replace.</param>
        /// <param name="value">The value to replace with.</param>
        public static void Replace<T>(this T[] source, T[] toChange, T value) {
            foreach (T entry in source) {
                foreach (T entryToChange in toChange) {
                    if (entry.Equals(entryToChange)) {
                        source[source.IndexOf(entry)] = value;
                        toChange = toChange.RemoveFirst();
                        break;
                    }
                }
                if (toChange.Length == 0) break;
            }
        }

        /// <summary>
        /// Replace multiple values within a Array. Replaces matching value 1 from toChange Array with the value 1 from values Array within the source Array.
        /// If the Array toChange is not in same size of the Array values, the function will throw a format exception and simple return.
        /// </summary>
        /// <typeparam name="T">The Type of the array to use and the value to add.</typeparam>
        /// <param name="source">The source Array.</param>
        /// <param name="toChange">The values to replace.</param>
        /// <param name="values">The values to replace with.</param>
        public static void ReplaceMulti<T>(this T[] source, T[] toChange, T[] values) {
            if (toChange.Length != values.Length) {
                throw new FormatException("Size Mismatch", new Exception("The Array toChange (Length " +
                                          toChange.Length.ToString() + ") is not in same size then the Array values (Length " + values.Length.ToString() + ")."));
            }
            foreach (T entry in source) {
                foreach (T entryToChange in toChange) {
                    if (entry.Equals(entryToChange)) {
                        int g = toChange.IndexOf(entryToChange);
                        source[source.IndexOf(entry)] = values[g];
                        toChange = toChange.Remove(g);
                        values = values.Remove(g);
                        break;
                    }
                }
                if (toChange.Length == 0) break;
            }
        }

        /// <summary>
        /// Replace a array with another one, within a array.
        /// </summary>
        /// <typeparam name="T">The Type of the array to use and the value to add.</typeparam>
        /// <param name="source">The source Array.</param>
        /// <param name="toChange">The array to replace.</param>
        /// <param name="patch">Thearray to patch.</param>
        public static void Replace<T>(this T[] source, T[] toChange, T[] patch) {
            if (source == null) throw new FormatException("Null Refernce", new Exception("The Array to check for a value existens is not Initialized."));
            if ((a = source.IndexOf(toChange)) != -1) {
                if (a + patch.Length <= source.Length) {
                    for (int i = 0; i < patch.Length; i++) source[a + i] = patch[i];
                } else {
                    throw new FormatException("Out of Range", new Exception("The Array to Replace oversize the source Array.\nArray end Offset right now: " +
                                              source.Length.ToString() + "\nEnd Offset after patch: " + (a + patch.Length).ToString()));
                }
            }
        }

        /// <summary>
        /// Replace a array within a array based on a indexer.
        /// </summary>
        /// <typeparam name="T">The Type of the array to use and the value to add.</typeparam>
        /// <param name="source">The source Array.</param>
        /// <param name="indexer">The offset within the array from where we shall start to patch.</param>
        /// <param name="patch">The array to patch.</param>
        public static void Replace<T>(this T[] source, int indexer, T[] patch) {
            if (source == null) throw new FormatException("Null Refernce", new Exception("The Array to check for a value existens is not Initialized."));
            if (indexer > -1) {
                if ((indexer + 1) + patch.Length <= source.Length) {
                    for (int i = 0; i < patch.Length; i++) source[indexer + i] = patch[i];
                } else {
                    throw new FormatException("Out of Range", new Exception("The Array to Replace oversize the source Array.\nArray end Offset right now: " +
                                              source.Length.ToString() + "\nEnd Offset after patch: " + (indexer + patch.Length).ToString()));
                }
            }
        }

        /// <summary>
        /// Replace a range of values within a Array of arrays.
        /// </summary>
        /// <typeparam name="T">The Type of the array to use and the value to add.</typeparam>
        /// <param name="source">The source Array.</param>
        /// <param name="toChange">The values to replace.</param>
        /// <param name="value">The value to replace with.</param>
        public static void Replace<T>(this T[][] source, T[] toChange, T[] value) { if ((a = source.IndexOf(toChange)) != -1) source[a] = value; }

        /// <summary>
        /// Replace a range of arrays within a Array of arrays.
        /// </summary>
        /// <typeparam name="T">The Type of the array to use and the value to add.</typeparam>
        /// <param name="source">The source Array.</param>
        /// <param name="toChange">The values to replace.</param>
        /// <param name="value">The values to replace with.</param>
        public static void Replace<T>(this T[][] source, T[][] toChange, T[] value) {
            foreach (T[] entry in source) {
                foreach (T[] entryToChange in toChange) {
                    if (entry.Equals(entryToChange)) {
                        source[source.IndexOf(entry)] = value;
                        toChange = toChange.RemoveFirst();
                        break;
                    }
                }
                if (toChange.Length == 0) break;
            }
        }

        /// <summary>
        /// Replace all matching values within a Array.
        /// </summary>
        /// <typeparam name="T">The Type of the array to use and the value to add.</typeparam>
        /// <param name="source">The source Array.</param>
        /// <param name="toChange">The value to replace.</param>
        /// <param name="value">The value to add.</param>
        public static void ReplaceAll<T>(this T[] source, T toChange, T value) {
            if (!source.Contains(toChange)) return;
            foreach (T entry in source) {
                if (entry.Equals(toChange)) {
                    source[source.IndexOf(entry)] = value;
                }
            }
        }

        /// <summary>
        /// Replace all matching values from a range within a Array.
        /// </summary>
        /// <typeparam name="T">The Type of the array to use and the value to add.</typeparam>
        /// <param name="source">The source Array.</param>
        /// <param name="toChange">The values to replace.</param>
        /// <param name="value">The value to replace with.</param>
        public static void ReplaceAll<T>(this T[] source, T[] toChange, T value) {
            foreach (T entry in source) {
                foreach (T entryToChange in toChange) {
                    if (entry.Equals(entryToChange)) {
                        source[source.IndexOf(entry)] = value;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Replace all matching arrays from a range off within a Array of arrays.
        /// </summary>
        /// <typeparam name="T">The Type of the array to use and the value to add.</typeparam>
        /// <param name="source">The source Array.</param>
        /// <param name="toChange">The values to replace.</param>
        /// <param name="value">The values to replace with.</param>
        public static void ReplaceAll<T>(this T[][] source, T[][] toChange, T[] value) {
            foreach (T[] entry in source) {
                foreach (T[] entryToChange in toChange) {
                    if (entry.Equals(entryToChange)) {
                        source[source.IndexOf(entry)] = value;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Remove a value from the Array.
        /// </summary>
        /// <typeparam name="T">The Type of the array to use and the value to remove.</typeparam>
        /// <param name="source">The source Array.</param>
        /// <param name="value">The value to remove.</param>
        /// /// <returns>The new Array with the added value.</returns>
        public static T[] Remove<T>(this T[] source, T value) {
            if ((a = source.IndexOf(value)) != -1) {
                T[] newArray = new T[source.Length - 1];
                for (int z = 0; z < source.Length; z++) if (z != a) newArray = newArray.Add(source[z]);
                return newArray;
            } else return source;
        }

        /// <summary>
        /// Remove a range of values from the Array.
        /// </summary>
        /// <typeparam name="T">The Type of the array to use and the value to remove.</typeparam>
        /// <param name="source">The source Array.</param>
        /// <param name="range">The values to remove.</param>
        /// /// <returns>The new Array with the added value.</returns>
        public static T[] RemoveMulti<T>(this T[] source, T[] range) {
            if (range.Length == 0) return source;
            T[] newArray = new T[0];
            bool found = false;
            foreach (T entry in source) {
                foreach (T toRemove in range) {
                    if (entry.Equals(toRemove)) {
                        found = true;
                        range = range.Remove(toRemove);
                        break;
                    }
                }
                if (!found) newArray = newArray.Add(entry);
                found = false;
            }
            return newArray;
        }

        /// <summary>
        /// Remove matching Bytes out of a byte Array.
        /// </summary>
        /// <param name="source">The source array to use.</param>
        /// <param name="remove">The bytes to remove.</param>
        /// /// <returns>The new Array with the added value.</returns>
        public static T[] Remove<T>(this T[] source, T[] remove) {
            if (source == null) throw new FormatException("Null Refernce", new Exception("The Array to check for a value existens is not Initialized."));
            if ((a = source.IndexOf(remove)) != -1) {
                T[] newArray = new T[source.Length - remove.Length];
                Array.Copy(source, 0, newArray, 0, a);
                Array.Copy(source, a + remove.Length, newArray, a, newArray.Length - a);
                return newArray;
            } else return source;
        }

        /// <summary>
        /// Remove all matching Bytes out of a byte Array.
        /// </summary>
        /// <param name="source">The source array to use.</param>
        /// <param name="remove">The bytes to remove.</param>
        /// /// <returns>The new Array with the added value.</returns>
        public static T[] RemoveAll<T>(this T[] source, T[] remove) {
            if (source == null) throw new FormatException("Null Refernce", new Exception("The Array to check for a value existens is not Initialized."));
            T[] work = source;
            while ((a = work.IndexOf(remove)) != -1) {
                T[] newArray = new T[source.Length - remove.Length];
                Array.Copy(source, 0, newArray, 0, a);
                Array.Copy(source, a + remove.Length, newArray, a, newArray.Length - a);
                work = newArray;
            }
            return work;
        }

        /// <summary>
        /// Remove a value from a Array by index.
        /// </summary>
        /// <typeparam name="T">The Type of the array to use.</typeparam>
        /// <param name="source">The source Array.</param>
        /// <param name="index">The index of the entry to remove.</param>
        /// /// <returns>The new Array with the added value.</returns>
        public static T[] Remove<T>(this T[] source, int index) {
            if (index + 1 > source.Length) return source;
            T[] newArray = new T[0];
            if (index + 1 > source.Length) { return source; }
            if (index == 0) { return source.RemoveFirst(); } else if (index + 1 == source.Length) { return source.RemoveLast(); } else { for (int i = 0; i < source.Length; i++) if (i != index) newArray = newArray.Add(source[i]); }
            return newArray;
        }

        /// <summary>
        /// Remove a range of values from a Array by index.
        /// </summary>
        /// <typeparam name="T">The Type of the array to use.</typeparam>
        /// <param name="source">The source Array.</param>
        /// <param name="index">The index of the entry from where to start to remove.</param>
        /// <param name="count">The amount of entrys to remove.</param>
        /// /// <returns>The new Array with the added value.</returns>
        public static T[] Remove<T>(this T[] source, int index, int count) {
            if (index + count > source.Length) return source;
            T[] newArray = new T[source.Length - count];
            if (newArray.Length == 0) { return newArray; }
            if (index == 0) Array.Copy(source, count, newArray, 0, newArray.Length);
            else if (index + count == source.Length) Array.Copy(source, newArray, source.Length - count);
            else {
                Array.Copy(source, newArray, index);
                Array.Copy(source, index + count, newArray, index, newArray.Length - index);
            }
            return newArray;
        }

        /// <summary>
        /// Remove a range of values from a Array by indexers.
        /// </summary>
        /// <typeparam name="T">The Type of the array to use.</typeparam>
        /// <param name="source">The source Array.</param>
        /// <param name="indexers">The indexers of the entrys to remove.</param>
        /// /// <returns>The new Array with the added value.</returns>
        public static T[] Remove<T>(this T[] source, int[] indexers) {
            foreach (int toCheck in indexers) if (toCheck + 1 > source.Length) indexers = indexers.Remove(toCheck);
            if (indexers.Length == 0) return source;
            T[] newArray = new T[0];
            Array.Sort(indexers);

            int count = 0;
            for (int i = 0; i < source.Length; i++) {
                if (i != indexers[count]) {
                    newArray = newArray.Add(source[i]);
                } else count++;
            }
            return newArray;
        }

        /// <summary>
        /// Remove a array from a Array of arrays by index.
        /// </summary>
        /// <typeparam name="T">The Type of the array to use.</typeparam>
        /// <param name="source">The source Array.</param>
        /// <param name="index">The index of the entry to remove.</param>
        /// /// <returns>The new Array with the added value.</returns>
        public static T[][] Remove<T>(this T[][] source, int index) {
            if (index + 1 > source.Length) return source;
            T[][] newArray = new T[0][];
            if (newArray.Length == 0) { return newArray; }
            if (index == 0) { return source.RemoveFirst(); } else if (index + 1 == source.Length) { return source.RemoveLast(); } else { for (int i = 0; i < source.Length; i++) if (i != index) newArray = newArray.Add(source[i]); }
            return newArray;
        }

        /// <summary>
        /// Remove a range of values from a Array of arrays by index.
        /// </summary>
        /// <typeparam name="T">The Type of the array to use.</typeparam>
        /// <param name="source">The source Array.</param>
        /// <param name="index">The index of the entry from where to start to remove.</param>
        /// <param name="count">The amount of entrys to remove.</param>
        /// /// <returns>The new Array with the added value.</returns>
        public static T[][] Remove<T>(this T[][] source, int index, int count) {
            if (index + count > source.Length && count <= source.Length) return source;
            T[][] newArray = new T[source.Length - count][];
            if (newArray.Length != 0) {
                if (index == 0) Array.Copy(source, count, newArray, 0, newArray.Length);
                else if (index + count == source.Length) Array.Copy(source, newArray, source.Length - count);
                else {
                    Array.Copy(source, newArray, index);
                    Array.Copy(source, index + count, newArray, index, newArray.Length - index);
                }
            }
            return newArray;
        }

        /// <summary>
        /// Remove a range of values from a Array of arrays by indexers.
        /// </summary>
        /// <typeparam name="T">The Type of the array to use.</typeparam>
        /// <param name="source">The source Array.</param>
        /// <param name="indexers">The indexers of the entrys to remove.</param>
        /// /// <returns>The new Array with the added value.</returns>
        public static T[][] Remove<T>(this T[][] source, int[] indexers) {
            foreach (int toCheck in indexers) if (toCheck + 1 > source.Length) indexers = indexers.Remove(toCheck);
            if (indexers.Length == 0) return source;
            T[][] newArray = new T[0][];
            Array.Sort(indexers);

            int count = 0;
            for (int i = 0; i < source.Length; i++) {
                if (i != indexers[count]) {
                    newArray = newArray.Add(source[i]);
                } else count++;
            }
            return newArray;
        }

        /// <summary>
        /// Remove a array from the Array of arrays.
        /// </summary>
        /// <typeparam name="T">The Type of the array to use and the value to remove.</typeparam>
        /// <param name="source">The source Array.</param>
        /// <param name="range">The array to remove.</param>
        /// /// <returns>The new Array with the added value.</returns>
        public static T[][] Remove<T>(this T[][] source, T[] range) {
            if ((a = source.IndexOf(range)) != -1) return source.Remove(a);
            return source;
        }

        /// <summary>
        /// Remove the first value from the Array.
        /// </summary>
        /// <typeparam name="T">The Type of the array to use.</typeparam>
        /// <param name="source">The source Array.</param>
        /// /// <returns>The new Array with the added value.</returns>
        public static T[] RemoveFirst<T>(this T[] source) { return source.Remove(0); }

        /// <summary>
        /// Remove the first array from the Array of arrays.
        /// </summary>
        /// <typeparam name="T">The Type of the array to use.</typeparam>
        /// <param name="source">The source Array.</param>
        /// /// <returns>The new Array with the added value.</returns>
        public static T[][] RemoveFirst<T>(this T[][] source) { return source.Remove(0); }

        /// <summary>
        /// Remove the first matching value from the Array.
        /// </summary>
        /// <typeparam name="T">The Type of the array to use.</typeparam>
        /// <param name="source">The source Array.</param>
        /// <param name="value">The value to remove from the array.</param>
        /// /// <returns>The new Array with the added value.</returns>
        public static T[] RemoveFirst<T>(this T[] source, T value) {
            if ((a = source.IndexOf(value)) != -1) return source.Remove(a);
            return source;
        }

        /// <summary>
        /// Remove the first matching array from the Array of arrays.
        /// </summary>
        /// <typeparam name="T">The Type of the array to use.</typeparam>
        /// <param name="source">The source Array.</param>
        /// <param name="value">The array to remove from the array of arrays.</param>
        /// /// <returns>The new Array with the added value.</returns>
        public static T[][] RemoveFirst<T>(this T[][] source, T[] value) {
            if ((a = source.IndexOf(value)) != -1) return source.Remove(a);
            return source;
        }

        /// <summary>
        /// Remove last value from the Array.
        /// </summary>
        /// <typeparam name="T">The Type of the array to use.</typeparam>
        /// <param name="source">The source Array.</param>
        /// /// <returns>The new Array with the added value.</returns>
        public static T[] RemoveLast<T>(this T[] source) { return source.Remove(source.Length - 1); }

        /// <summary>
        /// Remove last array from the Array of arrays.
        /// </summary>
        /// <typeparam name="T">The Type of the array to use.</typeparam>
        /// <param name="source">The source Array.</param>
        /// /// <returns>The new Array with the added value.</returns>
        public static T[][] RemoveLast<T>(this T[][] source) { return source.Remove(source.Length - 1); }

        /// <summary>
        /// Remove last matching value from the Array.
        /// </summary>
        /// <typeparam name="T">The Type of the array to use.</typeparam>
        /// <param name="source">The source Array.</param>
        /// <param name="value">The value to remove from the array.</param>
        /// /// <returns>The new Array with the added value.</returns>
        public static T[] RemoveLast<T>(this T[] source, T value) {
            if ((a = source.IndexOfLast(value)) != -1) return source.Remove(a);
            return source;
        }

        /// <summary>
        /// Remove last matching array from the Array of arrays.
        /// </summary>
        /// <typeparam name="T">The Type of the array to use.</typeparam>
        /// <param name="source">The source Array.</param>
        /// <param name="value">The array to remove from the array of arrays.</param>
        /// /// <returns>The new Array with the added value.</returns>
        public static T[][] RemoveLast<T>(this T[][] source, T[] value) {
            if ((a = source.IndexOfLast(value)) != -1) return source.Remove(a);
            return source;
        }

        /// <summary>
        /// Remove all matching values from the Array.
        /// </summary>
        /// <typeparam name="T">The Type of the array to use and the value to remove.</typeparam>
        /// <param name="source">The source Array.</param>
        /// <param name="value">The value to remove.</param>
        /// /// <returns>The new Array with the added value.</returns>
        public static T[] RemoveAll<T>(this T[] source, T value) {
            T[] remove = source;
            foreach (T entry in remove) if (entry.Equals(value)) remove = remove.Remove(entry);
            return remove;
        }

        /// <summary>
        /// Remove all empty entrys from a Array.
        /// </summary>
        /// <typeparam name="T">The Type of the array to use and the value to remove.</typeparam>
        /// <param name="source">The source Array to use.</param>
        public static T[] RemoveEmpty<T>(this T[] source) {
            int[] toRemove = new int[0];
            foreach (T entry in source) {
                if (typeof(T).IsValueType) {
                    if (typeof(T).Equals(typeof(string))) { if (entry.Equals(string.Empty)) toRemove.Add(source.IndexOf(entry)); } else if (typeof(T).Equals(typeof(ushort)) ||
                                                                                                                                            typeof(T).Equals(typeof(short)) ||
                                                                                                                                            typeof(T).Equals(typeof(uint)) ||
                                                                                                                                            typeof(T).Equals(typeof(int)) ||
                                                                                                                                            typeof(T).Equals(typeof(ulong)) ||
                                                                                                                                            typeof(T).Equals(typeof(long)) ||
                                                                                                                                            typeof(T).Equals(typeof(IntPtr)) ||
                                                                                                                                            typeof(T).Equals(typeof(UIntPtr)) ||
                                                                                                                                            typeof(T).Equals(typeof(byte)) ||
                                                                                                                                            typeof(T).Equals(typeof(decimal)) ||
                                                                                                                                            typeof(T).Equals(typeof(float)) ||
                                                                                                                                            typeof(T).Equals(typeof(double))) {
                        if (entry.Equals(0)) toRemove = toRemove.Add(source.IndexOf(entry));
                    } else MessagBox.Show("[ArrayExtension]\n[Function]:\nRemoveEmpty<T>(this T[] source);\nSorry this format is currently not Supported.\n" + typeof(T).ToString());
                } else if (default(T) == null) { if (entry.Equals(null)) toRemove.Add(source.IndexOf(entry)); } else MessagBox.Show("[ArrayExtension]\n[Function]:\nRemoveEmpty<T>(this T[] source);\nSorry this format is currently not Supported.\n" + typeof(T).ToString());
            }
            if (toRemove.Length > 0) return source.Remove(toRemove);
            return source;
        }

        /// <summary>
        /// Remove all empty arrays from a Array of Arrays.
        /// </summary>
        /// <typeparam name="T">The Type of the array to use and the value to remove.</typeparam>
        /// <param name="source">The source Array to use.</param>
        public static T[][] RemoveEmpty<T>(this T[][] source) {
            int[] toRemove = new int[0];
            foreach (T[] entry in source) { if (entry.Length == 0) toRemove = toRemove.Add(source.IndexOf(entry)); }
            if (toRemove.Length > 0) return source.Remove(toRemove);
            return source;
        }
    }

    /// <summary>
    /// BitConverter Extension.
    /// </summary>
    public static class BitConverterExtension {
        /// <summary>
        /// Get Bytes of the overloaded decimal value.
        /// </summary>
        /// <param name="dec">The decimal value to convert.</param>
        /// <returns>A byte array representing the decimal value.</returns>
        public static byte[] GetBytes(this decimal dec) {
            //Load four 32 bit integers from the Decimal.GetBits function
            Int32[] bits = decimal.GetBits(dec);

            //Create a temporary list to hold the bytes
            List<byte> bytes = new List<byte>();

            //iterate each 32 bit integer
            foreach (Int32 i in bits) bytes.AddRange(BitConverter.GetBytes(i)); //add the bytes of the current 32bit integer to the bytes list

            //return the bytes list as an array
            return bytes.ToArray();
        }

        /// <summary>
        /// Get Decimal value of the overloaded bytes.
        /// </summary>
        /// <param name="bytes">The bytes to convert.</param>
        /// <returns>A decimal value representing the convertet byte array.</returns>
        public static decimal ToDecimal(this byte[] bytes) {
            //check that it is even possible to convert the array
            if (bytes.Count() != 16) throw new Exception("A decimal must be created from exactly 16 bytes");

            //make an array to convert back to int32's
            Int32[] bits = new Int32[4];
            for (int i = 0; i <= 15; i += 4) bits[i / 4] = BitConverter.ToInt32(bytes, i); //convert every 4 bytes into an int32

            //Use the decimal's new constructor to
            //create an instance of decimal
            return new decimal(bits);
        }
    }

    /// <summary>
    /// Bitmap Extension.
    /// </summary>
    public static class BitmapExtension {
        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        public static Bitmap Resize(this Image image, int width, int height) {
            Rectangle newRect = new Rectangle(0, 0, width, height);
            Bitmap newImage = new Bitmap(width, height);

            newImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (Graphics graphics = Graphics.FromImage(newImage)) {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (ImageAttributes wrapMode = new ImageAttributes()) {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, newRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }
            return newImage;
        }
    }

    /// <summary>
    /// Anchor Styles shortages.
    /// </summary>
    public class Anch {
        /// <summary>
        /// Define to use All AnchorStyles.
        /// </summary>
        public static readonly AnchorStyles All = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

        /// <summary>
        /// Define to use only Bottom and Right AnchorStyle.
        /// </summary>
        public static readonly AnchorStyles BR = AnchorStyles.Bottom | AnchorStyles.Right;

        /// <summary>
        /// Define to use only Bottom and Left AnchorStyle.
        /// </summary>
        public static readonly AnchorStyles BL = AnchorStyles.Bottom | AnchorStyles.Left;

        /// <summary>
        /// Define to use only Bottom and Left AnchorStyle.
        /// </summary>
        public static readonly AnchorStyles BLR = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

        /// <summary>
        /// Define to use only Top and Left AnchorStyle.
        /// </summary>
        public static readonly AnchorStyles TL = AnchorStyles.Top | AnchorStyles.Left;

        /// <summary>
        /// Define to use only Top and Right AnchorStyle.
        /// </summary>
        public static readonly AnchorStyles TR = AnchorStyles.Top | AnchorStyles.Right;

        /// <summary>
        /// Define to use only Left and Right AnchorStyle.
        /// </summary>
        public static readonly AnchorStyles LR = AnchorStyles.Left | AnchorStyles.Right;

        /// <summary>
        /// Define to use Top, Left and Right AnchorStyle.
        /// </summary>
        public static readonly AnchorStyles TLR = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
    }

    /// <summary>
    /// Button Extension.
    /// </summary>
    public static class ButtonExtension {
        /// <summary>
        /// Center a Button.
        /// </summary>
        /// <param name="source">The Button to center.</param>
        /// <param name="parent">The Parent Control to center in.</param>
        public static void Center(this Button source, Control parent) { source.Location = new Point(((parent.ClientSize.Width / 2) - source.Size.Width / 2), source.Location.Y); }

        /// <summary>
        /// Center a range of Buttons.
        /// </summary>
        /// <param name="source">The Button to center.</param>
        /// <param name="parent">The Parent Control to center in.</param>
        /// <param name="otherButtons">The Other Buttons to Center too.</param>
        public static void Center(this Button source, Control parent, Button[] otherButtons) {
            int middleOfButtons = source.Size.Width;
            foreach (Button button in otherButtons) middleOfButtons += button.Size.Width + 5;
            middleOfButtons = middleOfButtons / 2;
            source.Location = new Point((parent.ClientSize.Width / 2) - middleOfButtons, source.Location.Y);
            Button toUse = source;
            foreach (Button button in otherButtons) {
                button.Location = new Point(toUse.Location.X + (toUse.Size.Width + 5), toUse.Location.Y);
                toUse = button;
            }
        }

        /// <summary>
        /// Allign a Button on the Left.
        /// </summary>
        /// <param name="source">The Button to allign on the Left.</param>
        /// <param name="parent">The Parent Control to allign in.</param>
        public static void Left(this Button source, Control parent) { source.Location = new Point(10, source.Location.Y); }

        /// <summary>
        /// Allign Buttons on the Left.
        /// </summary>
        /// <param name="source">The Button to allign on the Left.</param>
        /// <param name="parent">The Parent Control to allign in.</param>
        /// <param name="otherButtons">The Other Buttons to allign too.</param>
        public static void Left(this Button source, Control parent, Button[] otherButtons) {
            source.Left(parent);
            Button toUse = source;
            foreach (Button button in otherButtons) { button.Location = new Point(toUse.Location.X + toUse.Size.Width + 5, button.Location.Y); toUse = button; }
        }

        /// <summary>
        /// Allign Buttons on the Right.
        /// </summary>
        /// <param name="source">The Button to allign on the Right.</param>
        /// <param name="parent">The Parent Control to allign in.</param>
        public static void Right(this Button source, Control parent) { source.Location = new Point(((parent.ClientSize.Width - 10) - source.Size.Width), source.Location.Y); }

        /// <summary>
        /// Allign Buttons on the Right.
        /// </summary>
        /// <param name="source">The Button to allign on the Right.</param>
        /// <param name="parent">The Parent Control to allign in.</param>
        /// <param name="otherButtons">The Other Buttons to allign too.</param>
        public static void Right(this Button source, Control parent, Button[] otherButtons) {
            Button toUse = otherButtons[otherButtons.Length - 1];
            otherButtons[otherButtons.Length - 1].Right(parent);
            otherButtons.RemoveLast();
            if (otherButtons.Length > 1) Array.Reverse(otherButtons);
            otherButtons.Add(source);
            foreach (Button button in otherButtons) { button.Location = new Point(toUse.Location.X + toUse.Size.Width + 5, button.Location.Y); toUse = button; }
        }
    }

    /// <summary>
    /// A Custom Dialog Form which can be adjusted on wish.
    /// </summary>
    public class CustomDialog {
        #region Vars
        /// <summary>
        /// Controls for our custom Dialogs.
        /// </summary>
        private Form dialog;
        private Panel panel;
        private PictureBox pictureDialog;
        private Label label;
        private Button buttonOK;
        private Button buttonNo;
        private Button buttonCancel;

        /// <summary>
        /// A textBox for hte Input Dialog.
        /// </summary>
        public TextBox textBox;

        /// <summary>
        /// A CheckBox for the Custom Dialog.
        /// </summary>
        public CheckBox checkBox;

        /// <summary>
        /// Controls for the List Dialog.
        /// </summary>
        public static Form list;
        /// <summary>
        /// Button Add.
        /// </summary>
        public static Button buttonAdd;
        /// <summary>
        /// Button Rename.
        /// </summary>
        public static Button buttonRename;
        /// <summary>
        /// Button Show.
        /// </summary>
        public static Button buttonShow;
        /// <summary>
        /// Button Remove.
        /// </summary>
        public static Button buttonRemove;
        /// <summary>
        /// Button Ok for hte List dialog.
        /// </summary>
        public static Button buttonOKList;
        /// <summary>
        /// ListBox.
        /// </summary>
        public static ListBox listBox;
        /// <summary>
        /// EventHandler for clicks for the List Dialog.
        /// </summary>
        public static EventHandler[] ListClicks;

        /// <summary>
        /// The Size of the Dialog. (only used for the Input Dialog to adjust)
        /// </summary>
        public static Size Size = new Size(0, 0);

        /// <summary>
        /// Define a Font to use for the message text.
        /// </summary>
        public static Font Font = new Font("Century Gothic", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);

        /// <summary>
        /// The Icon for the Dialog to use.
        /// </summary>
        public static Icon DialogIcon = Resources.Empty;

        /// <summary>
        /// The Label aka Message Back Color.
        /// </summary>
        public static Color DialogBack = Color.White;

        /// <summary>
        /// The Label aka Message Fore Color.
        /// </summary>
        public static Color DialogFore = SystemColors.ControlText;

        /// <summary>
        /// The InputBox Back Color.
        /// </summary>
        public static Color TextBack = Color.White;

        /// <summary>
        /// The InputBox Fore Color.
        /// </summary>
        public static Color TextFore = SystemColors.ControlText;

        /// <summary>
        /// The Form Back Color.
        /// </summary>
        public static Color FormBack = SystemColors.Control;

        /// <summary>
        /// The Form Fore Color.
        /// </summary>
        public static Color FormFore = SystemColors.ControlText;

        /// <summary>
        /// A SoundPlayer for the Dialog.
        /// </summary>
        private static SoundPlayer soundPlayer = new SoundPlayer();

        /// <summary>
        /// 
        /// </summary>
        public string Message = string.Empty;

        /// <summary>
        /// Determine to use a Panel Control for the Dialog.
        /// </summary>
        public bool UsePanel = true;

        /// <summary>
        /// The Form AutoSize defination.
        /// </summary>
        public bool AutoSize = true;

        /// <summary>
        /// Show a Icon for this Dialog. (default is false)
        /// </summary>
        public bool ShowIcon = false;

        /// <summary>
        /// Determine to use Auto LineBreak on the Messages.
        /// </summary>
        public bool AutoLineBreak = false;

        /// <summary>
        /// Determine to use Icons for the buttons or not.
        /// </summary>
        public bool UseIcons = false;

        /// <summary>
        /// Reflects the checkBox checked state.
        /// </summary>
        public bool Checked = false;

        /// <summary>
        /// Tell the OnLoad Event to run a Sound.
        /// </summary>
        private bool playSound = false;

        /// <summary>
        /// Reflects the InputBox Text.
        /// </summary>
        public string UsrInput = string.Empty;

        /// <summary>
        /// Maximum Length for the User Input TextBox.
        /// </summary>
        public int MaxLength = 32767;

        /// <summary>
        /// Center the Form to his Parent by default.
        /// </summary>
        public FormStartPosition Start = FormStartPosition.CenterParent;

        /// <summary>
        /// Set Border Style to FixedSingle by Default.
        /// </summary>
        public FormBorderStyle Border = FormBorderStyle.FixedSingle;

        /// <summary>
        /// The position of the Cursor within the TextBox.
        /// </summary>
        public CursorPosition Cursor = CursorPosition.Right;

        /// <summary>
        /// The position of the buttons. Left, Center or Right. (default is Right)
        /// </summary>
        public ButtonPosition ButtonPosition = ButtonPosition.Right;
        #endregion Vars

        /// <summary>
        /// Initialize a basic Dialog Form.
        /// </summary>
        /// <param name="text">The Message Text.</param>
        /// <param name="caption">The Name of the Dialog.</param>
        public CustomDialog(string text, string caption) {
            // Initialize basic form.
            Initialize(text, caption);
        }

        /// <summary>
        /// Initialize a basic Dialog Form.
        /// </summary>
        /// <param name="textBox">Define to use a textBox.</param>
        /// <param name="text">The Message Text.</param>
        /// <param name="caption">The Name of the Dialog.</param>
        public CustomDialog(bool textBox, string message, string text, string caption) {
            // Initialize basic form.
            Initialize(text, caption);

            // Add a textBox for user input.
            AddTextBox(message);
        }

        /// <summary>
        /// Initialize a basic Dialog Form.
        /// </summary>
        /// <param name="textBox">Define to use a textBox.</param>
        /// <param name="text">The Message Text.</param>
        /// <param name="caption">The Name of the Dialog.</param>
        /// <param name="check">Define to use a checkbox.</param>
        public CustomDialog(bool textBox, string message, string text, string caption, string check) {
            // Initialize basic form.
            Initialize(text, caption);

            // Add a textBox for user input.
            AddTextBox(Message);

            // Add a CheckBox to the form.
            AddCheckBox(check);
        }

        /// <summary>
        /// Initialize a basic Dialog Form.
        /// </summary>
        /// <param name="textBox">Define to use a textBox.</param>
        /// <param name="text">The Message Text.</param>
        /// <param name="caption">The Name of the Dialog.</param>
        /// <param name="check">Define to use a checkbox.</param>
        /// <param name="checkState">Define the state of the checkbox.</param>
        public CustomDialog(bool textBox, string text, string caption, string check, bool checkState) {
            // Initialize basic form.
            Initialize(text, caption);

            // Add a textBox for user input.
            AddTextBox(Message);

            // Add a CheckBox to the form.
            AddCheckBox(check);
            checkBox.Checked = checkState;

        }

        /// <summary>
        /// Initialize a basic Dialog Form.
        /// </summary>
        /// <param name="text">The Message Text.</param>
        /// <param name="caption">The Name of the Dialog.</param>
        /// <param name="icon">The picture to display within the form.</param>
        public CustomDialog(string text, string caption, Image icon) {
            // Initialize basic form.
            Initialize(text, caption);

            // Add a Picture to the form.
            AddPicBox(icon);
        }

        /// <summary>
        /// Initialize a basic Dialog Form.
        /// </summary>
        /// <param name="text">The Message Text.</param>
        /// <param name="caption">The Name of the Dialog.</param>
        /// <param name="icon">The picture to display within the form.</param>
        /// /// <param name="check">The string of the checkbox to display.</param>
        public CustomDialog(string text, string caption, Image icon, string check) {
            // Initialize basic form.
            Initialize(text, caption);

            // Add a Picture to the form.
            AddPicBox(icon);

            // Add a CheckBox to the form.
            AddCheckBox(check);
        }

        /// <summary>
        /// Initialize a basic Dialog Form.
        /// </summary>
        /// <param name="text">The Message Text.</param>
        /// <param name="caption">The Name of the Dialog.</param>
        /// <param name="icon">The picture to display within the form.</param>
        /// <param name="check">The string of the checkbox to display.</param>
        /// <param name="checkState">The check state of the check box.</param>
        public CustomDialog(string text, string caption, Image icon, string check, bool checkState) {
            // Initialize basic form.
            Initialize(text, caption);

            // Add a Picture to the form.
            AddPicBox(icon);

            // Add a CheckBox to the form.
            AddCheckBox(check);
            checkBox.Checked = checkState;
        }

        /// <summary>
        /// Initialize a basic Dialog Form.
        /// </summary>
        /// <param name="text">The Message Text.</param>
        /// <param name="caption">The Name of the Dialog.</param>
        /// <param name="secondButton">Add a second Button to the form and define which button it shell be.</param> 
        public CustomDialog(string text, string caption, Buttons secondButton) {
            // Initialize basic form.
            Initialize(text, caption);

            // Add a second Button.
            AddSecondButton(secondButton);
        }

        /// <summary>
        /// Initialize a basic Dialog Form.
        /// </summary>
        /// <param name="text">The Message Text.</param>
        /// <param name="caption">The Name of the Dialog.</param>
        /// <param name="secondButton">Add a second Button to the form and define which button it shell be.</param> 
        /// <param name="icon">The picture to display within the form.</param>
        public CustomDialog(string text, string caption, Buttons secondButton, Image icon) {
            // Initialize basic form.
            Initialize(text, caption);

            // Add a second Button.
            AddSecondButton(secondButton);

            // Add a Picture to the form.
            AddPicBox(icon);
        }

        /// <summary>
        /// Initialize a basic Dialog Form.
        /// </summary>
        /// <param name="text">The Message Text.</param>
        /// <param name="caption">The Name of the Dialog.</param>
        /// <param name="secondButton">Add a second Button to the form and define which button it shell be.</param> 
        /// <param name="icon">The picture to display within the form.</param>
        /// <param name="sound">The sound to play.</param>
        public CustomDialog(string text, string caption, Buttons secondButton, Image icon, Stream sound) {
            // Initialize basic form.
            Initialize(text, caption);

            // Add a second Button.
            AddSecondButton(secondButton);

            // Add a Picture to the form.
            AddPicBox(icon);

            // Set the sound to play.
            if (sound != null) SetSound(sound);
        }

        /// <summary>
        /// Initialize a basic Dialog Form.
        /// </summary>
        /// <param name="text">The Message Text.</param>
        /// <param name="caption">The Name of the Dialog.</param>
        /// <param name="secondButton">Add a second Button to the form and define which button it shell be.</param> 
        /// <param name="icon">The picture to display within the form.</param>
        /// <param name="check">The string of the checkbox to display.</param>
        /// <param name="checkState">The check state of the check box.</param>
        public CustomDialog(string text, string caption, Buttons secondButton, Image icon, string check, bool checkState) {
            // Initialize basic form.
            Initialize(text, caption);

            // Add a second Button.
            AddSecondButton(secondButton);

            // Add a CheckBox to the form.
            AddCheckBox(check);
            checkBox.Checked = checkState;

            // Add a Picture to the form.
            AddPicBox(icon);
        }

        /// <summary>
        /// Initialize a basic Dialog Form.
        /// </summary>
        /// <param name="text">The Message Text.</param>
        /// <param name="caption">The Name of the Dialog.</param>
        /// <param name="secondButton">Add a second Button to the form and define which button it shell be.</param> 
        /// <param name="check">The string of the checkbox to display.</param>
        /// <param name="checkState">The check state of the check box.</param>
        public CustomDialog(string text, string caption, Buttons secondButton, string check, bool checkState) {
            // Initialize basic form.
            Initialize(text, caption);

            // Add a second Button.
            AddSecondButton(secondButton);

            // Add a CheckBox to the form.
            AddCheckBox(check);
            checkBox.Checked = checkState;
        }

        /// <summary>
        /// Initialize a basic Dialog Form.
        /// </summary>
        /// <param name="text">The Message Text.</param>
        /// <param name="caption">The Name of the Dialog.</param>
        /// <param name="check">The string of the checkbox to display.</param>
        public CustomDialog(string text, string caption, string check) {
            // Initialize basic form.
            Initialize(text, caption);

            // Add a CheckBox to the form.
            AddCheckBox(check);

            // Add a textBox for user input.
            if (Message != string.Empty) AddTextBox(Message);
        }

        /// <summary>
        /// Initialize a basic Dialog Form.
        /// </summary>
        /// <param name="text">The Message Text.</param>
        /// <param name="caption">The Name of the Dialog.</param>
        /// <param name="check">The string of the checkbox to display.</param>
        /// <param name="checkState">The check state of the check box.</param>
        public CustomDialog(string text, string caption, string check, bool checkState) {
            // Initialize basic form.
            Initialize(text, caption);

            // Add a CheckBox to the form.
            AddCheckBox(check);
            checkBox.Checked = checkState;

            // Add a textBox for user input.
            if (Message != string.Empty) AddTextBox(Message);
        }

        /// <summary>
        /// Initialize a basic Dialog Form.
        /// </summary>
        /// <param name="text">The Message Text.</param>
        /// <param name="caption">The Name of the Dialog.</param>
        /// <param name="sound">The sound to play.</param>
        public CustomDialog(string text, string caption, Stream sound) {
            // Initialize basic form.
            Initialize(text, caption);

            // Set the sound to play.
            if (sound != null) SetSound(sound);
        }

        /// <summary>
        /// Initialize a basic Dialog Form.
        /// </summary>
        /// <param name="text">The Message Text.</param>
        /// <param name="caption">The Name of the Dialog.</param>
        /// <param name="sound">The sound to play.</param>
        /// <param name="check">The string of the checkbox to display.</param>
        public CustomDialog(string text, string caption, Stream sound, string check) {
            // Initialize basic form.
            Initialize(text, caption);

            // Set the sound to play.
            if (sound != null) SetSound(sound);

            // Add a CheckBox to the form.
            AddCheckBox(check);
        }

        /// <summary>
        /// Initialize a basic Dialog Form.
        /// </summary>
        /// <param name="text">The Message Text.</param>
        /// <param name="caption">The Name of the Dialog.</param>
        /// <param name="sound">The sound to play.</param>
        /// <param name="icon">The picture to display within the form.</param>
        public CustomDialog(string text, string caption, Stream sound, Image icon) {
            // Initialize basic form.
            Initialize(text, caption);

            // Set the sound to play.
            if (sound != null) SetSound(sound);

            // Add a picture to the form.
            AddPicBox(icon);
        }

        /// <summary>
        /// Initialize a basic Dialog Form.
        /// </summary>
        /// <param name="text">The Message Text.</param>
        /// <param name="caption">The Name of the Dialog.</param>
        /// <param name="sound">The sound to play.</param>
        /// <param name="icon">The picture to display within the form.</param>
        /// <param name="check">The string of the checkbox to display.</param>
        /// <param name="checkState">The check state of the check box.</param>
        public CustomDialog(string text, string caption, Stream sound, Image icon, string check, bool checkState) {
            // Initialize basic form.
            Initialize(text, caption);

            // Set the sound to play.
            if (sound != null) SetSound(sound);

            // Add a picture to the form.
            AddPicBox(icon);

            // Add a CheckBox to the form.
            AddCheckBox(check);
            checkBox.Checked = checkState;
        }

        /// <summary>
        /// Initialize a basic Dialog Form.
        /// </summary>
        /// <param name="text">The Message Text.</param>
        /// <param name="caption">The Name of the Dialog.</param>
        /// <param name="sound">The sound to play.</param>
        /// <param name="icon">The picture to display within the form.</param>
        /// <param name="check">The string of the checkbox to display.</param>
        public CustomDialog(string text, string caption, Stream sound, Image icon, string check) {
            // Initialize basic form.
            Initialize(text, caption);

            // Set the sound to play.
            if (sound != null) SetSound(sound);

            // Add a picture to the form.
            AddPicBox(icon);

            // Add a CheckBox to the form.
            AddCheckBox(check);
        }

        /// <summary>
        /// Initialize a basic Dialog Form.
        /// </summary>
        /// <param name="text">The Message Text.</param>
        /// <param name="caption">The Name of the Dialog.</param>
        /// <param name="sound">The sound to play.</param>
        /// <param name="secondButton">Add a second Button to the form and define which button it shell be.</param>
        public CustomDialog(string text, string caption, Stream sound, Buttons secondButton) {
            // Initialize basic form.
            Initialize(text, caption);

            // Set the sound to play.
            if (sound != null) SetSound(sound);

            // Add a second Button.
            AddSecondButton(secondButton);
        }

        /// <summary>
        /// Initialize a basic Dialog Form.
        /// </summary>
        /// <param name="text">The Message Text.</param>
        /// <param name="caption">The Name of the Dialog.</param>
        /// <param name="sound">The sound to play.</param>
        /// <param name="secondButton">Add a second Button to the form and define which button it shell be.</param>
        /// <param name="icon">The picture to display within the form.</param>
        /// <param name="check">The string of the checkbox to display.</param>
        /// <param name="checkState">The check state of the check box.</param>
        public CustomDialog(string text, string caption, Stream sound, Buttons secondButton, Image icon, string check, bool checkState) {
            // Initialize basic form.
            Initialize(text, caption);

            // Set the sound to play.
            if (sound != null) SetSound(sound);

            // Add a second Button.
            AddSecondButton(secondButton);

            // Add a picture to the form.
            AddPicBox(icon);

            // Add a CheckBox to the form.
            AddCheckBox(check);
            checkBox.Checked = checkState;
        }

        /// <summary>
        /// Initialize a basic Dialog Form.
        /// </summary>
        /// <param name="text">The Message Text.</param>
        /// <param name="caption">The Name of the Dialog.</param>
        /// <param name="sound">The sound to play.</param>
        /// <param name="secondButton">Add a second Button to the form and define which button it shell be.</param>
        /// <param name="check">The string of the checkbox to display.</param>
        public CustomDialog(string text, string caption, Stream sound, Buttons secondButton, string check) {
            // Initialize basic form.
            Initialize(text, caption);

            // Set the sound to play.
            if (sound != null) SetSound(sound);

            // Add a second Button.
            AddSecondButton(secondButton);

            // Add a CheckBox to the form.
            AddCheckBox(check);
        }

        /// <summary>
        /// Initialize a basic Dialog Form.
        /// </summary>
        /// <param name="text">The Message Text.</param>
        /// <param name="caption">The Name of the Dialog.</param>
        /// <param name="sound">The sound to play.</param>
        /// <param name="secondButton">Add a second Button to the form and define which button it shell be.</param>
        /// <param name="check">The string of the checkbox to display.</param>
        /// <param name="checkState">The check state of the check box.</param>
        public CustomDialog(string text, string caption, Stream sound, Buttons secondButton, string check, bool checkState) {
            // Initialize basic form.
            Initialize(text, caption);

            // Set the sound to play.
            if (sound != null) SetSound(sound);

            // Add a second Button.
            AddSecondButton(secondButton);

            // Add a CheckBox to the form.
            AddCheckBox(check);
            checkBox.Checked = checkState;
        }

        /// <summary>
        /// Initialize a basic Dialog Form.
        /// </summary>
        /// <param name="text">The Message Text.</param>
        /// <param name="caption">The Name of the Dialog.</param>
        /// <param name="sound">The sound to play.</param>
        /// <param name="check">The string of the checkbox to display.</param>
        /// <param name="checkState">The check state of the check box.</param>
        public CustomDialog(string text, string caption, Stream sound, string check, bool checkState) {
            // Initialize basic form.
            Initialize(text, caption);

            // Set the sound to play.
            if (sound != null) SetSound(sound);

            // Add a CheckBox to the form.
            AddCheckBox(check);
            checkBox.Checked = checkState;
        }

        /// <summary>
        /// Initialize the form.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="caption"></param>
        private void Initialize(string text, string caption) {
            // Clear the check and the Text Box, also in case we use it to determine which dialog do actually run.
            checkBox = null;
            textBox = null;
            pictureDialog = null;
            buttonNo = buttonCancel = null;

            // Initialize Components, Draw the Form.
            if (UsePanel) panel = new Panel() { Anchor = Anch.All, BackColor = DialogBack, Location = new Point(0, 0), Size = new Size(137, 66), TabIndex = 0 };
            label = new Label() { Anchor = Anch.TL, AutoSize = true, Font = Font, ForeColor = DialogFore, Text = text, Location = new Point(12, 27), Size = new Size(38, 15), TabIndex = 0 };
            buttonOK = new Button() { Anchor = Anch.BR, Location = new Point(41, 79), Size = new Size(86, 26), TabIndex = 1, Text = "OK", UseVisualStyleBackColor = true };
            buttonOK.Click += (sender, e) => { dialog.DialogResult = DialogResult.OK; };
            dialog = new Form() {
                AutoScaleDimensions = new SizeF(6F, 13F), AutoScaleMode = AutoScaleMode.Font, ClientSize = new Size(137, 116), MaximizeBox = false, MinimizeBox = false,
                ShowIcon = ShowIcon, Icon = DialogIcon, StartPosition = Start, FormBorderStyle = Border, Text = caption, BackColor = FormBack, ForeColor = FormFore
            };

            // Shall we use Icons ?
            if (UseIcons) {
                buttonOK.Text = string.Empty;
                buttonOK.Image = (Image)Resources.ResourceManager.GetObject("Check");
            }

            // Define Click Events.
            dialog.Load += Dialog_Load;

            // Throw all Components together.
            if (UsePanel) {
                panel.Controls.Add(label);
                dialog.Controls.Add(panel);
            } else dialog.Controls.Add(label);
            dialog.Controls.Add(buttonOK);
        }

        /// <summary>
        /// Add a CheckBox to the Dialog.
        /// </summary>
        /// <param name="text">The CheckBox Text.</param>
        private void AddCheckBox(string text) {
            checkBox = new CheckBox() {
                Anchor = Anch.BL, AutoSize = true, Text = text, Font = new Font("Neo Sans", 8.239999F), Location = new Point(12, 47), Size = new Size(15, 14), TabIndex = 2, UseVisualStyleBackColor = true
            };
            if (UsePanel) panel.Controls.Add(checkBox);
            else dialog.Controls.Add(checkBox);
        }

        /// <summary>
        /// Add a TextBox to the Dialog.
        /// </summary>
        /// <param name="text">The TextBox Text.</param>
        private void AddTextBox(string text) {
            textBox = new TextBox() { Anchor = Anch.TLR, Font = Font, BackColor = TextBack, ForeColor = TextFore, Location = new Point(15, 45), Size = new Size(145, 21), TabIndex = 3, Text = text, MaxLength = MaxLength };
            dialog.ClientSize = new Size(170, 146);
            if (UsePanel) panel.Controls.Add(textBox);
            else dialog.Controls.Add(textBox);
            if (Size.Width != 0 && Size.Height != 0) dialog.ClientSize = new Size(Size.Width, Size.Height);
            UsrInput = string.Empty;
        }

        /// <summary>
        /// Add a PictureBox to the dialog which will be used as icon box.
        /// </summary>
        /// <param name="picture">The icon to use.</param>
        private void AddPicBox(Image picture) {
            if (picture.Size.Width > 32 || picture.Size.Height > 32) picture = picture.Resize(32, 32);
            pictureDialog = new PictureBox() {
                Anchor = Anch.TL, Image = picture, Location = new Point(27, 26), Size = new Size(32, 32), TabIndex = 3, TabStop = false,
                SizeMode = PictureBoxSizeMode.CenterImage
            };
            dialog.Size = new Size(214, 183);
            label.Location = new Point(64, 34);
            label.Anchor = Anch.TL;
            if (UsePanel) panel.Controls.Add(pictureDialog);
            else dialog.Controls.Add(pictureDialog);
        }

        /// <summary>
        /// Add a second Button to the Dialog.
        /// </summary>
        /// <param name="button">Buttons to use.</param>
        private void AddSecondButton(Buttons button) {
            if (button == Buttons.Ok) return;
            else if (button == Buttons.Yes) { buttonOK.Text = "YES"; return; }

            buttonNo = new Button() { Anchor = Anch.BR, Location = new Point(132, 79), Size = new Size(86, 26), TabIndex = 2, Text = "NO", UseVisualStyleBackColor = true };
            buttonNo.Click += (sender, e) => { dialog.DialogResult = DialogResult.No; };
            dialog.ClientSize = new Size(226, 116);
            buttonOK.Location = new Point(41, 79);
            dialog.Controls.Add(buttonNo);
            if (button == Buttons.YesNo) {
                buttonOK.Text = "YES";
                buttonOK.Click -= (sender, e) => { dialog.DialogResult = DialogResult.OK; };
                buttonOK.Click += (sender, e) => { dialog.DialogResult = DialogResult.Yes; };
            } else if (button == Buttons.OkNoCancel || button == Buttons.YesNoCancel) AddThirdButton(button);
        }

        /// <summary>
        /// Add a third Button to the Dialog.
        /// </summary>
        /// <param name="button">Buttons to use.</param>
        private void AddThirdButton(Buttons button) {
            buttonCancel = new Button() { Anchor = Anch.BR, Location = new Point(224, 79), Size = new Size(86, 26), TabIndex = 3, Text = "Cancel", UseVisualStyleBackColor = true };
            buttonCancel.Click += (sender, e) => { dialog.DialogResult = DialogResult.Cancel; };
            dialog.ClientSize = new Size(319, 116);
            buttonOK.Location = new Point(41, 79);
            buttonNo.Location = new Point(132, 79);
            dialog.Controls.Add(buttonCancel);
            if (button == Buttons.YesNoCancel) {
                buttonOK.Text = "YES";
                buttonOK.Click -= (sender, e) => { dialog.DialogResult = DialogResult.OK; };
                buttonOK.Click += (sender, e) => { dialog.DialogResult = DialogResult.Yes; };
            }
        }

        /// <summary>
        /// Set the SoundPlayer Stream to the new Stream to play and set flag for the OnLoad Event.
        /// </summary>
        /// <param name="sound">The Sound to set.</param>
        private void SetSound(Stream sound) {
            soundPlayer.Stream = sound;
            playSound = true;
        }

        /// <summary>
        /// On Load of the Dialog Form do.
        /// </summary>
        /// <param name="sender">The Sender.</param>
        /// <param name="e">The Event Arguments.</param>
        public void Dialog_Load(object sender, EventArgs e) {
            // Set a Custom Icon for this Dialog.
            if (ShowIcon && DialogIcon != null) dialog.Icon = DialogIcon;

            // Adjust TextBox if used.
            if (textBox != null) {
                if (Cursor == CursorPosition.Right) textBox.SelectionStart = textBox.Text.Length;
                else if (Cursor == CursorPosition.Left) textBox.SelectionStart = 0;
            }

            // Adjust Text if AutoLineBreak is used.
            if (AutoLineBreak) {
                if (!label.Text.Contains('\n')) {
                    if (label.Text.Length > 72) {
                        string sub = string.Empty;
                        int z = 0;
                        int x = 0;
                        int Count = 72;
                        int textLength = label.Text.Length;
                        int toRead = label.Text.Length;
                        for (int i = 0; i < textLength; i += Count) {
                            if (toRead < 72) sub = label.Text.Substring(x, toRead);
                            else sub = label.Text.Substring(x, 72);
                            if (!sub.Contain(".")) {
                                if (!sub.Contain("?")) {
                                    if (!sub.Contain("!")) {
                                        if (!sub.Contain(" ")) {
                                            label.Text = label.Text.Insert(x + 72, "\n");
                                            z = 72;
                                        } else {
                                            z = sub.LastIndexOf(" ") + 1;
                                            label.Text = label.Text.Insert(z, "\n");
                                        }
                                    } else {
                                        z = sub.LastIndexOf("!") + 1;
                                        label.Text = label.Text.Insert(z, "\n");
                                    }
                                } else {
                                    z = sub.LastIndexOf("?") + 1;
                                    label.Text = label.Text.Insert(z, "\n");
                                }
                            } else {
                                z = sub.LastIndexOf(".") + 1;
                                label.Text = label.Text.Insert(z, "\n");
                            }

                            toRead -= z;
                            x += (z + 1);
                            Count = (z + 1);
                        }
                    }
                }
            }

            // Adjust Dialog Form to match the text message.
            if (pictureDialog == null && textBox == null) dialog.ClientSize = new Size(label.Size.Width + 23, label.Size.Height + 101);
            else if (pictureDialog != null && textBox == null) {
                string[] adjust = label.Text.Split('\n');
                int widthNew = label.Size.Width + 92;
                if (widthNew < 164) widthNew = 165;
                dialog.ClientSize = new Size(widthNew, label.Size.Height + 107);
                pictureDialog.Location = new Point(pictureDialog.Location.X, pictureDialog.Location.Y + ((adjust.Length - 1) * 5));
            }

            // Adjust CheckBox if used.
            if (checkBox != null) {
                dialog.ClientSize = new Size(dialog.ClientSize.Width, dialog.ClientSize.Height + 12);
                int length = checkBox.Size.Width - dialog.ClientSize.Width;
                if (length > 0) dialog.ClientSize = new Size(dialog.ClientSize.Width + (length + 20), dialog.ClientSize.Height);
            }

            // Adjust buttons.
            if (buttonNo != null && buttonCancel == null) { if (dialog.ClientSize.Width < 226) dialog.ClientSize = new Size(226, dialog.ClientSize.Height); } else if (buttonNo != null && buttonCancel != null) { if (dialog.ClientSize.Width < 319) dialog.ClientSize = new Size(319, dialog.ClientSize.Height); }
            if (ButtonPosition != ButtonPosition.Right) {
                if (ButtonPosition == ButtonPosition.Left) {
                    if (buttonNo != null && buttonCancel == null) {
                        buttonOK.Anchor = buttonNo.Anchor = Anch.BL;
                        buttonOK.Left(dialog, new Button[] { buttonNo });
                    } else if (buttonNo != null && buttonCancel != null) {
                        buttonOK.Anchor = buttonNo.Anchor = buttonCancel.Anchor = Anch.BL;
                        buttonOK.Left(dialog, new Button[] { buttonNo, buttonCancel });
                    } else {
                        buttonOK.Anchor = Anch.BL;
                        buttonOK.Left(dialog);
                    }
                } else {
                    if (buttonNo != null && buttonCancel == null) {
                        buttonOK.Anchor = buttonNo.Anchor = AnchorStyles.Bottom;
                        buttonOK.Center(dialog, new Button[] { buttonNo });
                    } else if (buttonNo != null && buttonCancel != null) {
                        buttonOK.Anchor = buttonNo.Anchor = buttonCancel.Anchor = AnchorStyles.Bottom;
                        buttonOK.Center(dialog, new Button[] { buttonNo, buttonCancel });
                    } else {
                        buttonOK.Anchor = AnchorStyles.Bottom;
                        buttonOK.Center(dialog);
                    }
                }
            }

            // Play a sound for this dialog if set.
            if (playSound) {
                soundPlayer.Play();
                playSound = false;
            }
        }

        /// <summary>
        /// Piped 'ShowDialog()' function.
        /// </summary>
        public DialogResult ShowDialog() { return dialog.ShowDialog(); }
    }

    /// <summary>
    /// A MessageBox Wrapper with predifend Casts.
    /// </summary>
    public static class MessagBox {
        #region Vars
        /// <summary>
        /// Error Image for the Dialog.
        /// </summary>
        public static Image ErrorImage = Resources.Error.Resize(32, 32);

        /// <summary>
        /// Info Image for the Dialog.
        /// </summary>
        public static Image InfoImage = Resources.Info.Resize(32, 32);

        /// <summary>
        /// Alert Image for the Dialog.
        /// </summary>
        public static Image WarningImage = Resources.Alert.Resize(32, 32);

        /// <summary>
        /// Question Image for the Dialog.
        /// </summary>
        public static Image QuestionImage = Resources.Question.Resize(32, 32);

        /// <summary>
        /// Debug Image for the Dialog.
        /// </summary>
        public static Image DebugImage = Resources.DbgClamp.Resize(32, 32);

        /// <summary>
        /// Defien a custome Size to adjust the message dialog.
        /// </summary>
        public static Size Size = new Size(0, 0);

        /// <summary>
        /// Determine to use a Panel Control for the Dialog.
        /// </summary>
        public static bool UsePanel = true;

        /// <summary>
        /// The Form AutoSize defination.
        /// </summary>
        public static bool AutoSize = true;

        /// <summary>
        /// Show a Icon for this Dialog. (default is false)
        /// </summary>
        public static bool ShowIcon = false;

        /// <summary>
        /// Determine to use Auto LineBreak on the Messages.
        /// </summary>
        public static bool AutoLineBreak = false;

        /// <summary>
        /// Determine to use Icons for the buttons or not.
        /// </summary>
        public static bool UseIcons = false;

        /// <summary>
        /// Center the Form to his Parent by default.
        /// </summary>
        public static FormStartPosition Start = FormStartPosition.CenterParent;

        /// <summary>
        /// Set Border Style to FixedSingle by Default.
        /// </summary>
        public static FormBorderStyle Border = FormBorderStyle.FixedSingle;

        /// <summary>
        /// The position of the Cursor within the TextBox.
        /// </summary>
        public static CursorPosition Cursor = CursorPosition.Right;

        /// <summary>
        /// The position of the buttons. Left, Center or Right. (default is Right)
        /// </summary>
        public static ButtonPosition ButtonPosition = ButtonPosition.Right;

        /// <summary>
        /// Reflects the checkBox checked state.
        /// </summary>
        public static bool Checked = false;

        /// <summary>
        /// Reflects the InputBox Text.
        /// </summary>
        public static string UsrInput = string.Empty;

        /// <summary>
        /// The InputBox Back Color.
        /// </summary>
        public static Color TextBack = Color.White;

        /// <summary>
        /// The InputBox Fore Color.
        /// </summary>
        public static Color TextFore = SystemColors.ControlText;
        #endregion Vars

        #region Functions
        /// <summary>
        /// Reset the public Class vars to their standart.
        /// </summary>
        public static void Reset() {
            Start = FormStartPosition.CenterParent;
            Border = FormBorderStyle.FixedSingle;
            Cursor = CursorPosition.Right;
            ButtonPosition = ButtonPosition.Right;
            AutoSize = UsePanel = true;
            ShowIcon = AutoLineBreak = UseIcons = false;
        }

        /// <summary>
        /// A Simple message dialog.
        /// </summary>
        /// <param name="text">The message to display.</param>
        /// <param name="args">Addional arguments for the message.</param>
        /// <returns>The Dialog Result.</returns>
        public static void Show(string text, [Optional] params object[] args) {
            if (args != null) text = text.FormatString(args);
            CustomDialog cDialog = new CustomDialog(text, string.Empty) {
                Start = Start, Border = Border, Cursor = Cursor, ButtonPosition = ButtonPosition, AutoSize = AutoSize, UsePanel = UsePanel, ShowIcon = ShowIcon, AutoLineBreak = AutoLineBreak, UseIcons = UseIcons
            };
            cDialog.ShowDialog();
        }

        /// <summary>
        /// A Simple message dialog.
        /// </summary>
        /// <param name="caption">The title to display.</param>
        /// <param name="text">The message to display.</param>
        /// <param name="args">Addional arguments for the message.</param>
        /// <returns>The Dialog Result.</returns>
        public static void Show(string caption, string text, [Optional] params object[] args) {
            if (args != null) text = text.FormatString(args);
            CustomDialog cDialog = new CustomDialog(text, caption) {
                Start = Start, Border = Border, Cursor = Cursor, ButtonPosition = ButtonPosition, AutoSize = AutoSize, UsePanel = UsePanel, ShowIcon = ShowIcon, AutoLineBreak = AutoLineBreak, UseIcons = UseIcons
            };
            cDialog.ShowDialog();
        }

        /// <summary>
        /// A Simple message dialog.
        /// </summary>
        /// <param name="button">Define other buttons to use.</param>
        /// <param name="caption">The title to display.</param>
        /// <param name="text">The message to display.</param>
        /// <param name="args">Addional arguments for the message.</param>
        /// <returns>The Dialog Result.</returns>
        public static DialogResult Show(Buttons button, string caption, string text, [Optional] params object[] args) {
            if (args != null) text = text.FormatString(args);
            CustomDialog cDialog = new CustomDialog(text, caption, button) {
                Start = Start, Border = Border, Cursor = Cursor, ButtonPosition = ButtonPosition, AutoSize = AutoSize, UsePanel = UsePanel, ShowIcon = ShowIcon, AutoLineBreak = AutoLineBreak, UseIcons = UseIcons
            };
            return cDialog.ShowDialog();
        }

        /// <summary>
        /// A Simple message dialog with a CheckBox added.
        /// </summary>
        /// <param name="check">The Text of the CheckBox to display.</param>
        /// <param name="caption">The title to display.</param>
        /// <param name="text">The message to display.</param>
        /// <param name="args">Addional arguments for the message.</param>
        /// <returns>The Dialog Result.</returns>
        public static DialogResult Show(string check, string caption, string text, [Optional] params object[] args) {
            if (args != null) text = text.FormatString(args);
            CustomDialog cDialog = new CustomDialog(text, caption, check) {
                Start = Start, Border = Border, Cursor = Cursor, ButtonPosition = ButtonPosition, AutoSize = AutoSize, UsePanel = UsePanel, ShowIcon = ShowIcon, AutoLineBreak = AutoLineBreak, UseIcons = UseIcons
            };
            DialogResult result = cDialog.ShowDialog();
            Checked = cDialog.checkBox.Checked;
            return result;
        }

        /// <summary>
        /// A Simple message dialog with a CheckBox added.
        /// </summary>
        /// <param name="check">The Text of the CheckBox to display.</param>
        /// <param name="checkState">Define the checkBox checked state.</param>
        /// <param name="caption">The title to display.</param>
        /// <param name="text">The message to display.</param>
        /// <param name="args">Addional arguments for the message.</param>
        /// <returns>The Dialog Result.</returns>
        public static DialogResult Show(string check, bool checkState, string caption, string text, [Optional] params object[] args) {
            if (args != null) text = text.FormatString(args);
            CustomDialog cDialog = new CustomDialog(text, caption, check, checkState) {
                Start = Start, Border = Border, Cursor = Cursor, ButtonPosition = ButtonPosition, AutoSize = AutoSize, UsePanel = UsePanel, ShowIcon = ShowIcon, AutoLineBreak = AutoLineBreak, UseIcons = UseIcons
            };
            DialogResult result = cDialog.ShowDialog();
            Checked = cDialog.checkBox.Checked;
            return result;
        }

        /// <summary>
        /// A Simple message dialog with a CheckBox added.
        /// </summary>
        /// <param name="button">Define other buttons to use.</param>
        /// <param name="check">The Text of the CheckBox to display.</param>
        /// <param name="checkState">Define the checkBox checked state.</param>
        /// <param name="caption">The title to display.</param>
        /// <param name="text">The message to display.</param>
        /// <param name="args">Addional arguments for the message.</param>
        /// <returns>The Dialog Result.</returns>
        public static DialogResult Show(Buttons button, string check, bool checkState, string caption, string text, [Optional] params object[] args) {
            if (args != null) text = text.FormatString(args);
            CustomDialog cDialog = new CustomDialog(text, caption, button, check, checkState) {
                Start = Start, Border = Border, Cursor = Cursor, ButtonPosition = ButtonPosition, AutoSize = AutoSize, UsePanel = UsePanel, ShowIcon = ShowIcon, AutoLineBreak = AutoLineBreak, UseIcons = UseIcons
            };
            DialogResult result = cDialog.ShowDialog();
            Checked = cDialog.checkBox.Checked;
            return result;
        }

        /// <summary>
        /// A Simple message dialog.
        /// </summary>
        /// <param name="icon">The icon or image to show within the dialog.</param>
        /// <param name="text">The message to display.</param>
        /// <param name="args">Addional arguments for the message.</param>
        /// <returns>The Dialog Result.</returns>
        public static void Show(Image icon, string text, [Optional] params object[] args) {
            if (args != null) text = text.FormatString(args);
            CustomDialog cDialog = new CustomDialog(text, string.Empty, icon) {
                Start = Start, Border = Border, Cursor = Cursor, ButtonPosition = ButtonPosition, AutoSize = AutoSize, UsePanel = UsePanel, ShowIcon = ShowIcon, AutoLineBreak = AutoLineBreak, UseIcons = UseIcons
            };
            cDialog.ShowDialog();
        }

        /// <summary>
        /// A Simple message dialog.
        /// </summary>
        /// <param name="icon">The icon or image to show within the dialog.</param>
        /// <param name="caption">The title to display.</param>
        /// <param name="text">The message to display.</param>
        /// <param name="args">Addional arguments for the message.</param>
        /// <returns>The Dialog Result.</returns>
        public static void Show(Image icon, string caption, string text, [Optional] params object[] args) {
            if (args != null) text = text.FormatString(args);
            CustomDialog cDialog = new CustomDialog(text, caption, icon) {
                Start = Start, Border = Border, Cursor = Cursor, ButtonPosition = ButtonPosition, AutoSize = AutoSize, UsePanel = UsePanel, ShowIcon = ShowIcon, AutoLineBreak = AutoLineBreak, UseIcons = UseIcons
            };
            cDialog.ShowDialog();
        }

        /// <summary>
        /// A Simple message dialog.
        /// </summary>
        /// <param name="icon">The icon or image to show within the dialog.</param>
        /// <param name="button">Define other buttons to use.</param>
        /// <param name="caption">The title to display.</param>
        /// <param name="text">The message to display.</param>
        /// <param name="args">Addional arguments for the message.</param>
        /// <returns>The Dialog Result.</returns>
        public static DialogResult Show(Image icon, Buttons button, string caption, string text, [Optional] params object[] args) {
            if (args != null) text = text.FormatString(args);
            CustomDialog cDialog = new CustomDialog(text, caption, button, icon) {
                Start = Start, Border = Border, Cursor = Cursor, ButtonPosition = ButtonPosition, AutoSize = AutoSize, UsePanel = UsePanel, ShowIcon = ShowIcon, AutoLineBreak = AutoLineBreak, UseIcons = UseIcons
            };
            return cDialog.ShowDialog();
        }

        /// <summary>
        /// A Simple message dialog with a CheckBox added.
        /// </summary>
        /// <param name="icon">The icon or image to show within the dialog.</param>
        /// <param name="check">The Text of the CheckBox to display.</param>
        /// <param name="caption">The title to display.</param>
        /// <param name="text">The message to display.</param>
        /// <param name="args">Addional arguments for the message.</param>
        /// <returns>The Dialog Result.</returns>
        public static DialogResult Show(Image icon, string check, string caption, string text, [Optional] params object[] args) {
            if (args != null) text = text.FormatString(args);
            CustomDialog cDialog = new CustomDialog(text, caption, icon, check) {
                Start = Start, Border = Border, Cursor = Cursor, ButtonPosition = ButtonPosition, AutoSize = AutoSize, UsePanel = UsePanel, ShowIcon = ShowIcon, AutoLineBreak = AutoLineBreak, UseIcons = UseIcons
            };
            DialogResult result = cDialog.ShowDialog();
            Checked = cDialog.checkBox.Checked;
            return result;
        }

        /// <summary>
        /// A Simple message dialog with a CheckBox added.
        /// </summary>
        /// <param name="icon">The icon or image to show within the dialog.</param>
        /// <param name="check">The Text of the CheckBox to display.</param>
        /// <param name="checkState">Define the checkBox checked state.</param>
        /// <param name="caption">The title to display.</param>
        /// <param name="text">The message to display.</param>
        /// <param name="args">Addional arguments for the message.</param>
        /// <returns>The Dialog Result.</returns>
        public static DialogResult Show(Image icon, string check, bool checkState, string caption, string text, [Optional] params object[] args) {
            if (args != null) text = text.FormatString(args);
            CustomDialog cDialog = new CustomDialog(text, caption, icon, check, checkState) {
                Start = Start, Border = Border, Cursor = Cursor, ButtonPosition = ButtonPosition, AutoSize = AutoSize, UsePanel = UsePanel, ShowIcon = ShowIcon, AutoLineBreak = AutoLineBreak, UseIcons = UseIcons
            };
            DialogResult result = cDialog.ShowDialog();
            Checked = cDialog.checkBox.Checked;
            return result;
        }

        /// <summary>
        /// A Simple message dialog with a CheckBox added.
        /// </summary>
        /// <param name="icon">The icon or image to show within the dialog.</param>
        /// <param name="button">Define other buttons to use.</param>
        /// <param name="check">The Text of the CheckBox to display.</param>
        /// <param name="checkState">Define the checkBox checked state.</param>
        /// <param name="caption">The title to display.</param>
        /// <param name="text">The message to display.</param>
        /// <param name="args">Addional arguments for the message.</param>
        /// <returns>The Dialog Result.</returns>
        public static DialogResult Show(Image icon, Buttons button, string check, bool checkState, string caption, string text, [Optional] params object[] args) {
            if (args != null) text = text.FormatString(args);
            CustomDialog cDialog = new CustomDialog(text, caption, button, icon, check, checkState) {
                Start = Start, Border = Border, Cursor = Cursor, ButtonPosition = ButtonPosition, AutoSize = AutoSize, UsePanel = UsePanel, ShowIcon = ShowIcon, AutoLineBreak = AutoLineBreak, UseIcons = UseIcons
            };
            DialogResult result = cDialog.ShowDialog();
            Checked = cDialog.checkBox.Checked;
            return result;
        }

        /// <summary>
        /// A Simple message dialog.
        /// </summary>
        /// <param name="sound">The sound to play for this dialog.</param>
        /// <param name="text">The message to display.</param>
        /// <param name="args">Addional arguments for the message.</param>
        /// <returns>The Dialog Result.</returns>
        public static void Show(Stream sound, string text, [Optional] params object[] args) {
            if (args != null) text = text.FormatString(args);
            CustomDialog cDialog = new CustomDialog(text, string.Empty, sound) {
                Start = Start, Border = Border, Cursor = Cursor, ButtonPosition = ButtonPosition, AutoSize = AutoSize, UsePanel = UsePanel, ShowIcon = ShowIcon, AutoLineBreak = AutoLineBreak, UseIcons = UseIcons
            };
            cDialog.ShowDialog();
        }

        /// <summary>
        /// A Simple message dialog.
        /// </summary>
        /// <param name="sound">The sound to play for this dialog.</param>
        /// <param name="caption">The title to display.</param>
        /// <param name="text">The message to display.</param>
        /// <param name="args">Addional arguments for the message.</param>
        /// <returns>The Dialog Result.</returns>
        public static void Show(Stream sound, string caption, string text, [Optional] params object[] args) {
            if (args != null) text = text.FormatString(args);
            CustomDialog cDialog = new CustomDialog(text, caption, sound) {
                Start = Start, Border = Border, Cursor = Cursor, ButtonPosition = ButtonPosition, AutoSize = AutoSize, UsePanel = UsePanel, ShowIcon = ShowIcon, AutoLineBreak = AutoLineBreak, UseIcons = UseIcons
            };
            cDialog.ShowDialog();
        }

        /// <summary>
        /// A Simple message dialog.
        /// </summary>
        /// <param name="sound">The sound to play for this dialog.</param>
        /// <param name="button">Define other buttons to use.</param>
        /// <param name="caption">The title to display.</param>
        /// <param name="text">The message to display.</param>
        /// <param name="args">Addional arguments for the message.</param>
        /// <returns>The Dialog Result.</returns>
        public static DialogResult Show(Stream sound, Buttons button, string caption, string text, [Optional] params object[] args) {
            if (args != null) text = text.FormatString(args);
            CustomDialog cDialog = new CustomDialog(text, caption, sound, button) {
                Start = Start, Border = Border, Cursor = Cursor, ButtonPosition = ButtonPosition, AutoSize = AutoSize, UsePanel = UsePanel, ShowIcon = ShowIcon, AutoLineBreak = AutoLineBreak, UseIcons = UseIcons
            };
            return cDialog.ShowDialog();
        }

        /// <summary>
        /// A Simple message dialog with a CheckBox added.
        /// </summary>
        /// <param name="sound">The sound to play for this dialog.</param>
        /// <param name="check">The Text of the CheckBox to display.</param>
        /// <param name="caption">The title to display.</param>
        /// <param name="text">The message to display.</param>
        /// <param name="args">Addional arguments for the message.</param>
        /// <returns>The Dialog Result.</returns>
        public static DialogResult Show(Stream sound, string check, string caption, string text, [Optional] params object[] args) {
            if (args != null) text = text.FormatString(args);
            CustomDialog cDialog = new CustomDialog(text, caption, sound, check) {
                Start = Start, Border = Border, Cursor = Cursor, ButtonPosition = ButtonPosition, AutoSize = AutoSize, UsePanel = UsePanel, ShowIcon = ShowIcon, AutoLineBreak = AutoLineBreak, UseIcons = UseIcons
            };
            DialogResult result = cDialog.ShowDialog();
            Checked = cDialog.checkBox.Checked;
            return result;
        }

        /// <summary>
        /// A Simple message dialog with a CheckBox added.
        /// </summary>
        /// <param name="sound">The sound to play for this dialog.</param>
        /// <param name="check">The Text of the CheckBox to display.</param>
        /// <param name="checkState">Define the checkBox checked state.</param>
        /// <param name="caption">The title to display.</param>
        /// <param name="text">The message to display.</param>
        /// <param name="args">Addional arguments for the message.</param>
        /// <returns>The Dialog Result.</returns>
        public static DialogResult Show(Stream sound, string check, bool checkState, string caption, string text, [Optional] params object[] args) {
            if (args != null) text = text.FormatString(args);
            CustomDialog cDialog = new CustomDialog(text, caption, sound, check, checkState) {
                Start = Start, Border = Border, Cursor = Cursor, ButtonPosition = ButtonPosition, AutoSize = AutoSize, UsePanel = UsePanel, ShowIcon = ShowIcon, AutoLineBreak = AutoLineBreak, UseIcons = UseIcons
            };
            DialogResult result = cDialog.ShowDialog();
            Checked = cDialog.checkBox.Checked;
            return result;
        }

        /// <summary>
        /// A Simple message dialog with a CheckBox added.
        /// </summary>
        /// <param name="sound">The sound to play for this dialog.</param>
        /// <param name="button">Define other buttons to use.</param>
        /// <param name="check">The Text of the CheckBox to display.</param>
        /// <param name="checkState">Define the checkBox checked state.</param>
        /// <param name="caption">The title to display.</param>
        /// <param name="text">The message to display.</param>
        /// <param name="args">Addional arguments for the message.</param>
        /// <returns>The Dialog Result.</returns>
        public static DialogResult Show(Stream sound, Buttons button, string check, bool checkState, string caption, string text, [Optional] params object[] args) {
            if (args != null) text = text.FormatString(args);
            CustomDialog cDialog = new CustomDialog(text, caption, sound, button, check, checkState) {
                Start = Start, Border = Border, Cursor = Cursor, ButtonPosition = ButtonPosition, AutoSize = AutoSize, UsePanel = UsePanel, ShowIcon = ShowIcon, AutoLineBreak = AutoLineBreak, UseIcons = UseIcons
            };
            DialogResult result = cDialog.ShowDialog();
            Checked = cDialog.checkBox.Checked;
            return result;
        }

        /// <summary>
        /// A Simple message dialog.
        /// </summary>
        /// <param name="icon">The icon or image to show within the dialog.</param>
        /// <param name="sound">The sound to play for this dialog.</param>
        /// <param name="text">The message to display.</param>
        /// <param name="args">Addional arguments for the message.</param>
        /// <returns>The Dialog Result.</returns>
        public static void Show(Image icon, Stream sound, string text, [Optional] params object[] args) {
            if (args != null) text = text.FormatString(args);
            CustomDialog cDialog = new CustomDialog(text, string.Empty, sound, icon) {
                Start = Start, Border = Border, Cursor = Cursor, ButtonPosition = ButtonPosition, AutoSize = AutoSize, UsePanel = UsePanel, ShowIcon = ShowIcon, AutoLineBreak = AutoLineBreak, UseIcons = UseIcons
            };
            cDialog.ShowDialog();
        }

        /// <summary>
        /// A Simple message dialog.
        /// </summary>
        /// <param name="icon">The icon or image to show within the dialog.</param>
        /// <param name="sound">The sound to play for this dialog.</param>
        /// <param name="caption">The title to display.</param>
        /// <param name="text">The message to display.</param>
        /// <param name="args">Addional arguments for the message.</param>
        /// <returns>The Dialog Result.</returns>
        public static void Show(Image icon, Stream sound, string caption, string text, [Optional] params object[] args) {
            if (args != null) text = text.FormatString(args);
            CustomDialog cDialog = new CustomDialog(text, caption, sound, icon) {
                Start = Start, Border = Border, Cursor = Cursor, ButtonPosition = ButtonPosition, AutoSize = AutoSize, UsePanel = UsePanel, ShowIcon = ShowIcon, AutoLineBreak = AutoLineBreak, UseIcons = UseIcons
            };
            cDialog.ShowDialog();
        }

        /// <summary>
        /// A Simple message dialog.
        /// </summary>
        /// <param name="icon">The icon or image to show within the dialog.</param>
        /// <param name="sound">The sound to play for this dialog.</param>
        /// <param name="button">Define other buttons to use.</param>
        /// <param name="caption">The title to display.</param>
        /// <param name="text">The message to display.</param>
        /// <param name="args">Addional arguments for the message.</param>
        /// <returns>The Dialog Result.</returns>
        public static DialogResult Show(Image icon, Stream sound, Buttons button, string caption, string text, [Optional] params object[] args) {
            if (args != null) text = text.FormatString(args);
            CustomDialog cDialog = new CustomDialog(text, caption, button, icon, sound) {
                Start = Start, Border = Border, Cursor = Cursor, ButtonPosition = ButtonPosition, AutoSize = AutoSize, UsePanel = UsePanel, ShowIcon = ShowIcon, AutoLineBreak = AutoLineBreak, UseIcons = UseIcons
            };
            return cDialog.ShowDialog();
        }

        /// <summary>
        /// A Simple message dialog with a CheckBox added.
        /// </summary>
        /// <param name="icon">The icon or image to show within the dialog.</param>
        /// <param name="sound">The sound to play for this dialog.</param>
        /// <param name="check">The Text of the CheckBox to display.</param>
        /// <param name="caption">The title to display.</param>
        /// <param name="text">The message to display.</param>
        /// <param name="args">Addional arguments for the message.</param>
        /// <returns>The Dialog Result.</returns>
        public static DialogResult Show(Image icon, Stream sound, string check, string caption, string text, [Optional] params object[] args) {
            if (args != null) text = text.FormatString(args);
            CustomDialog cDialog = new CustomDialog(text, caption, sound, icon, check) {
                Start = Start, Border = Border, Cursor = Cursor, ButtonPosition = ButtonPosition, AutoSize = AutoSize, UsePanel = UsePanel, ShowIcon = ShowIcon, AutoLineBreak = AutoLineBreak, UseIcons = UseIcons
            };
            DialogResult result = cDialog.ShowDialog();
            Checked = cDialog.checkBox.Checked;
            return result;
        }

        /// <summary>
        /// A Simple message dialog with a CheckBox added.
        /// </summary>
        /// <param name="icon">The icon or image to show within the dialog.</param>
        /// <param name="sound">The sound to play for this dialog.</param>
        /// <param name="check">The Text of the CheckBox to display.</param>
        /// <param name="checkState">Define the checkBox checked state.</param>
        /// <param name="caption">The title to display.</param>
        /// <param name="text">The message to display.</param>
        /// <param name="args">Addional arguments for the message.</param>
        /// <returns>The Dialog Result.</returns>
        public static DialogResult Show(Image icon, Stream sound, string check, bool checkState, string caption, string text, [Optional] params object[] args) {
            if (args != null) text = text.FormatString(args);
            CustomDialog cDialog = new CustomDialog(text, caption, sound, icon, check, checkState) {
                Start = Start, Border = Border, Cursor = Cursor, ButtonPosition = ButtonPosition, AutoSize = AutoSize, UsePanel = UsePanel, ShowIcon = ShowIcon, AutoLineBreak = AutoLineBreak, UseIcons = UseIcons
            };
            DialogResult result = cDialog.ShowDialog();
            Checked = cDialog.checkBox.Checked;
            return result;
        }

        /// <summary>
        /// A Simple message dialog with a CheckBox added.
        /// </summary>
        /// <param name="icon">The icon or image to show within the dialog.</param>
        /// <param name="sound">The sound to play for this dialog.</param>
        /// <param name="button">Define other buttons to use.</param>
        /// <param name="check">The Text of the CheckBox to display.</param>
        /// <param name="checkState">Define the checkBox checked state.</param>
        /// <param name="caption">The title to display.</param>
        /// <param name="text">The message to display.</param>
        /// <param name="args">Addional arguments for the message.</param>
        /// <returns>The Dialog Result.</returns>
        public static DialogResult Show(Image icon, Stream sound, Buttons button, string check, bool checkState, string caption, string text, [Optional] params object[] args) {
            if (args != null) text = text.FormatString(args);
            CustomDialog cDialog = new CustomDialog(text, caption, sound, button, icon, check, checkState) {
                Start = Start, Border = Border, Cursor = Cursor, ButtonPosition = ButtonPosition, AutoSize = AutoSize, UsePanel = UsePanel, ShowIcon = ShowIcon, AutoLineBreak = AutoLineBreak, UseIcons = UseIcons
            };
            DialogResult result = cDialog.ShowDialog();
            Checked = cDialog.checkBox.Checked;
            return result;
        }

        /// <summary>
        /// A Input Dialog.
        /// </summary>
        /// <param name="message">The message to display.</param>
        /// <param name="args">Addional arguments for the message.</param>
        /// <returns>The Dialog Result.</returns>
        public static DialogResult Input(string message, [Optional] params object[] args) {
            if (args != null) message = message.FormatString(args);
            CustomDialog.Size = Size;
            CustomDialog.TextBack = TextBack;
            CustomDialog.TextFore = TextFore;
            CustomDialog cDialog = new CustomDialog(true, message, string.Empty, string.Empty) {
                Start = Start, Border = Border, Cursor = Cursor, ButtonPosition = ButtonPosition, AutoSize = AutoSize, UsePanel = UsePanel, ShowIcon = ShowIcon, AutoLineBreak = AutoLineBreak, UseIcons = UseIcons
            };
            DialogResult result = cDialog.ShowDialog();
            UsrInput = cDialog.textBox.Text;
            return result;
        }

        /// <summary>
        /// A Input Dialog.
        /// </summary>
        /// <param name="message">The message to display.</param>
        /// <param name="text">The label Text.</param>
        /// <param name="args">Addional arguments for the message.</param>
        ///<returns>The Dialog Result.</returns>
        public static DialogResult Input(string message, string text, [Optional] params object[] args) {
            if (args != null) message = message.FormatString(args);
            CustomDialog.Size = Size;
            CustomDialog.TextBack = TextBack;
            CustomDialog.TextFore = TextFore;
            CustomDialog cDialog = new CustomDialog(true, message, text, string.Empty) {
                Start = Start, Border = Border, Cursor = Cursor, ButtonPosition = ButtonPosition, AutoSize = AutoSize, UsePanel = UsePanel, ShowIcon = ShowIcon, AutoLineBreak = AutoLineBreak, UseIcons = UseIcons
            };
            DialogResult result = cDialog.ShowDialog();
            UsrInput = cDialog.textBox.Text;
            return result;
        }

        /// <summary>
        /// A Input Dialog.
        /// </summary>
        /// <param name="message">The message to display.</param>
        /// <param name="text">The label Text.</param>
        /// <param name="caption">The Title of the Dialog.</param>
        /// <param name="args">Addional arguments for the message.</param>
        /// <returns>The Dialog Result.</returns>
        public static DialogResult Input(string message, string text, string caption, [Optional] params object[] args) {
            if (args != null) message = message.FormatString(args);
            CustomDialog.Size = Size;
            CustomDialog.TextBack = TextBack;
            CustomDialog.TextFore = TextFore;
            CustomDialog cDialog = new CustomDialog(true, message, text, caption) {
                Start = Start, Border = Border, Cursor = Cursor, ButtonPosition = ButtonPosition, AutoSize = AutoSize, UsePanel = UsePanel, ShowIcon = ShowIcon, AutoLineBreak = AutoLineBreak, UseIcons = UseIcons
            };
            DialogResult result = cDialog.ShowDialog();
            UsrInput = cDialog.textBox.Text;
            return result;
        }

        /// <summary>
        /// A Input Dialog.
        /// </summary>
        /// <param name="message">The message to display.</param>
        /// <param name="text">The label Text.</param>
        /// <param name="caption">The Title of the Dialog.</param>
        /// <param name="args">Addional arguments for the message.</param>
        /// <param name="maxLength">The maximum characters of the input string.</param>
        /// <returns>The Dialog Result.</returns>
        public static DialogResult Input(string message, string text, string caption, int maxLength, [Optional] params object[] args) {
            if (args != null) message = message.FormatString(args);
            CustomDialog.Size = Size;
            CustomDialog.TextBack = TextBack;
            CustomDialog.TextFore = TextFore;
            CustomDialog cDialog = new CustomDialog(true, message, text, caption) {
                Start = Start, Border = Border, Cursor = Cursor, ButtonPosition = ButtonPosition, AutoSize = AutoSize, UsePanel = UsePanel, ShowIcon = ShowIcon, AutoLineBreak = AutoLineBreak, UseIcons = UseIcons,
                MaxLength = maxLength,
            };            
            DialogResult result = cDialog.ShowDialog();
            UsrInput = cDialog.textBox.Text;
            return result;
        }

        /// <summary>
        /// A Input Dialog with a CheckBox added.
        /// </summary>
        /// <param name="check">The Text of the CheckBox to display.</param>
        /// <param name="message">The message to display.</param>
        /// <param name="text">The label Text.</param>
        /// <param name="caption">The Title of the Dialog.</param>
        /// <param name="args">Addional arguments for the message.</param>
        /// <returns>The Dialog Result.</returns>
        public static DialogResult Input(string check, string message, string text, string caption, [Optional] params object[] args) {
            if (args != null) message = message.FormatString(args);
            CustomDialog.Size = Size;
            CustomDialog.TextBack = TextBack;
            CustomDialog.TextFore = TextFore;
            CustomDialog cDialog = new CustomDialog(true, text, caption, check) {
                Start = Start, Border = Border, Cursor = Cursor, ButtonPosition = ButtonPosition, AutoSize = AutoSize, UsePanel = UsePanel, ShowIcon = ShowIcon, AutoLineBreak = AutoLineBreak, UseIcons = UseIcons, Message = message
            };
            DialogResult result = cDialog.ShowDialog();
            UsrInput = cDialog.textBox.Text;
            Checked = cDialog.checkBox.Checked;
            return result;
        }

        /// <summary>
        /// A Input Dialog with a CheckBox added.
        /// </summary>
        /// <param name="check">The Text of the CheckBox to display.</param>
        /// <param name="CheckState">Define the checkBox checked state.</param>
        /// <param name="message">The message to display.</param>
        /// <param name="text">The label Text.</param>
        /// <param name="caption">The Title of the Dialog.</param>
        /// <param name="args">Addional arguments for the message.</param>
        /// <returns>The Dialog Result.</returns>
        public static DialogResult Input(string check, bool CheckState, string message, string text, string caption, [Optional] params object[] args) {
            if (args != null) message = message.FormatString(args);
            CustomDialog.Size = Size;
            CustomDialog.TextBack = TextBack;
            CustomDialog.TextFore = TextFore;
            CustomDialog cDialog = new CustomDialog(true, text, caption, check, CheckState) {
                Start = Start, Border = Border, Cursor = Cursor, ButtonPosition = ButtonPosition, AutoSize = AutoSize, UsePanel = UsePanel, ShowIcon = ShowIcon, AutoLineBreak = AutoLineBreak, UseIcons = UseIcons, Message = message
            };
            DialogResult result = cDialog.ShowDialog();
            UsrInput = cDialog.textBox.Text;
            Checked = cDialog.checkBox.Checked;
            return result;
        }

        /// <summary>
        /// Show a error dialog, specifing a title.
        /// </summary>
        /// <param name="title">The Title of hte MessageBox Dialog.</param>
        /// <param name="message">The message to show.</param>
        /// <param name="args">Addional arguments for the message.</param>
        public static void Error(string title, string message, [Optional] params object[] args) {
            if (args != null) message = message.FormatString(args);
            Show(ErrorImage, Resources.ErrorSound, Buttons.Ok, title, message);
        }

        /// <summary>
        /// Show a error dialog, specifing a title.
        /// </summary>
        /// <param name="message">The message to show.</param>
        /// <param name="args">Addional arguments for the message.</param>
        public static void Error(string message, [Optional] params object[] args) {
            if (args != null) message = message.FormatString(args);
            Show(ErrorImage, Resources.ErrorSound, Buttons.Ok, "", message);
        }

        /// <summary>
        /// Show a error dialog, specifing a title.
        /// </summary>
        /// <param name="button">Define other buttons to use.</param>
        /// <param name="title">The Title of hte MessageBox Dialog.</param>
        /// <param name="message">The message to show.</param>
        /// <param name="args">Addional arguments for the message.</param>
        public static DialogResult Error(Buttons button, string title, string message, [Optional] params object[] args) {
            if (args != null) message = message.FormatString(args);
            return Show(ErrorImage, Resources.ErrorSound, button, title, message);
        }

        /// <summary>
        /// Show a error dialog, specifing a title.
        /// </summary>
        /// <param name="button">Define other buttons to use.</param>
        /// <param name="message">The message to show.</param>
        /// <param name="args">Addional arguments for the message.</param>
        public static DialogResult Error(Buttons button, string message, [Optional] params object[] args) {
            if (args != null) message = message.FormatString(args);
            return Show(ErrorImage, Resources.ErrorSound, button, "", message);
        }

        /// <summary>
        /// Show a info dialog, specifing a title.
        /// </summary>
        /// <param name="title">The Title of hte MessageBox Dialog.</param>
        /// <param name="message">The message to show.</param>
        /// <param name="args">Addional arguments for the message.</param>
        public static void Info(string title, string message, [Optional] params object[] args) {
            if (args != null) message = message.FormatString(args);
            Show(InfoImage, Resources.InfoSound, Buttons.Ok, title, message);
        }

        /// <summary>
        /// Show a info dialog.
        /// </summary>
        /// <param name="message">The message to show.</param>
        /// /// <param name="args">Addional arguments for the message.</param>
        public static void Info(string message, [Optional] params object[] args) {
            if (args != null) message = message.FormatString(args);
            Show(InfoImage, Resources.InfoSound, Buttons.Ok, "", message);
        }

        /// <summary>
        /// Show a info dialog, specifing a title.
        /// </summary>
        /// <param name="button">Define other buttons to use.</param>
        /// <param name="title">The Title of hte MessageBox Dialog.</param>
        /// <param name="message">The message to show.</param>
        /// <param name="args">Addional arguments for the message.</param>
        public static DialogResult Info(Buttons button, string title, string message, [Optional] params object[] args) {
            if (args != null) message = message.FormatString(args);
            return Show(InfoImage, Resources.InfoSound, button, title, message);
        }

        /// <summary>
        /// Show a info dialog.
        /// </summary>
        /// <param name="button">Define other buttons to use.</param>
        /// <param name="message">The message to show.</param>
        /// <param name="args">Addional arguments for the message.</param>
        public static DialogResult Info(Buttons button, string message, [Optional] params object[] args) {
            if (args != null) message = message.FormatString(args);
            return Show(InfoImage, Resources.InfoSound, button, "", message);
        }

        /// <summary>
        /// Show a warning, specifing a title.
        /// </summary>
        /// <param name="title">The Title of hte MessageBox Dialog.</param>
        /// <param name="message">The message to show.</param>
        /// /// <param name="args">Addional arguments for the message.</param>
        public static void Warning(string title, string message, [Optional] params object[] args) {
            if (args != null) message = message.FormatString(args);
            Show(WarningImage, Resources.AlertSound, Buttons.Ok, title, message);
        }

        /// <summary>
        /// Show a warning.
        /// </summary>
        /// <param name="message">The message to show.</param>
        /// /// <param name="args">Addional arguments for the message.</param>
        public static void Warning(string message, [Optional] params object[] args) {
            if (args != null) message = message.FormatString(args);
            Show(WarningImage, Resources.AlertSound, Buttons.Ok, "", message);
        }

        /// <summary>
        /// Show a warning, specifing a title.
        /// </summary>
        /// <param name="button">Buttons to use.</param>
        /// <param name="title">The Title of hte MessageBox Dialog.</param>
        /// <param name="message">The message to show.</param>
        /// <param name="args">Addional arguments for the message.</param>
        public static DialogResult Warning(Buttons button, string title, string message, [Optional] params object[] args) {
            if (args != null) message = message.FormatString(args);
            return Show(WarningImage, Resources.AlertSound, button, title, message);
        }

        /// <summary>
        /// Show a warning.
        /// </summary>
        /// <param name="button">Buttons to use.</param>
        /// <param name="message">The message to show.</param>
        /// <param name="args">Addional arguments for the message.</param>
        public static DialogResult Warning(Buttons button, string message, [Optional] params object[] args) {
            if (args != null) message = message.FormatString(args);
            return Show(WarningImage, Resources.AlertSound, button, "", message);
        }

        /// <summary>
        /// Show a Question dialog, specifing a title.
        /// </summary>
        /// <param name="title">The Title of hte MessageBox Dialog.</param>
        /// <param name="message">The message to show.</param>
        /// /// <param name="args">Addional arguments for the message.</param>
        /// <returns>The Dialog Result.</returns>
        public static DialogResult Question(string title, string message, [Optional] params object[] args) {
            if (args != null) message = message.FormatString(args);
            return Show(QuestionImage, Resources.QuestionSound, Buttons.OkNo, title, message);
        }

        /// <summary>
        /// Show a Question dialog.
        /// </summary>
        /// <param name="message">The message to show.</param>
        /// <param name="args">Addional arguments for the message.</param>
        /// <returns>The Dialog Result.</returns>
        public static DialogResult Question(string message, [Optional] params object[] args) {
            if (args != null) message = message.FormatString(args);
            return Show(QuestionImage, Resources.QuestionSound, Buttons.OkNo, "", message);
        }

        /// <summary>
        /// Show a question dialog, specifing a title, with 3 buttons, Yes, No and Cancel.
        /// </summary>
        /// <param name="button">Buttons to use.</param>
        /// <param name="title">The Title of hte MessageBox Dialog.</param>
        /// <param name="message">The message to show.</param>
        /// <param name="args">Addional arguments for the message.</param>
        /// <returns>The Dialog Result.</returns>
        public static DialogResult Question(Buttons button, string title, string message, [Optional] params object[] args) {
            if (args != null) message = message.FormatString(args);
            return Show(QuestionImage, Resources.QuestionSound, button, title, message);
        }

        /// <summary>
        /// Show a question dialog with 3 buttons, Yes, No and Cancel.
        /// </summary>
        /// <param name="button">Buttons to use.</param>
        /// <param name="message">The message to show.</param>
        /// <param name="args">Addional arguments for the message.</param>
        /// <returns>The Dialog Result.</returns>
        public static DialogResult Question(Buttons button, string message, [Optional] params object[] args) {
            if (args != null) message = message.FormatString(args);
            return Show(QuestionImage, Resources.QuestionSound, button, "", message);
        }

        /// <summary>
        /// Show a Debug Message.
        /// </summary>
        /// <param name="message">The Message to display.</param>
        /// <param name="args">Addional arguments for the message.</param>
        public static void Debug(string message, [Optional] params object[] args) {
            if (args != null) message = message.FormatString(args);
            Show(DebugImage, Resources.DebugSound, message);
        }

        /// <summary>
        /// Show a Debug Message.
        /// </summary>
        /// <param name="caption">A Title for the Message.</param>
        /// <param name="message">The Message to display.</param>
        /// <param name="args">Addional arguments for the message.</param>
        public static void Debug(string caption, string message, [Optional] params object[] args) {
            if (args != null) message = message.FormatString(args);
            Show(DebugImage, Resources.DebugSound, caption, message);
        }

        /// <summary>
        /// Show a basic OpenFile Dialog, specifing a tile and/or a initial directory.
        /// </summary>
        /// <param name="title">The Title to display.</param>
        /// <param name="filter">The file filter to use as string.</param>
        /// <param name="initialDirectory">The initial directory to sue for this dialog.</param>
        /// <returns>The user input as string or a empty string.</returns>
        public static string ShowOpenFile(string title, string filter, string initialDirectory) {
            string result = string.Empty;
            OpenFileDialog openFile = new OpenFileDialog();
            if (title != string.Empty) openFile.Title = title;
            if (filter != string.Empty) openFile.Filter = filter;
            if (initialDirectory != string.Empty) openFile.InitialDirectory = initialDirectory;
            else openFile.InitialDirectory = Directory.GetCurrentDirectory();
            openFile.Multiselect = false;
            openFile.AddExtension = true;
            openFile.CheckFileExists = true;
            if (openFile.ShowDialog() == DialogResult.OK) result = openFile.FileName;
            return result;
        }

        /// <summary>
        /// Show a basic OpenFile Dialog, specifing a tile and/or a initial directory.
        /// </summary>
        /// <param name="title">The Title to display.</param>
        /// <param name="filter">The file filter to use as string.</param>
        /// <param name="name">The name for the file to display.</param>
        /// <param name="initialDirectory">The initial directory to sue for this dialog.</param>
        /// <returns>The user input as string or a empty string.</returns>
        public static string ShowOpenFile(string title, string filter, string name, string initialDirectory) {
            string result = string.Empty;
            OpenFileDialog openFile = new OpenFileDialog();
            if (title != string.Empty) openFile.Title = title;
            if (filter != string.Empty) openFile.Filter = filter;
            if (name != string.Empty) openFile.FileName = name;
            if (initialDirectory != string.Empty) openFile.InitialDirectory = initialDirectory;
            else openFile.InitialDirectory = Directory.GetCurrentDirectory();
            openFile.Multiselect = false;
            openFile.AddExtension = true;
            openFile.CheckFileExists = true;
            if (openFile.ShowDialog() == DialogResult.OK) result = openFile.FileName;
            return result;
        }

        /// <summary>
        /// Show a basic OpenFile Dialog.
        /// </summary>
        /// <returns>The user input as string or a empty string.</returns>
        public static string ShowOpenFile() {
            string result = string.Empty;
            OpenFileDialog openFile = new OpenFileDialog() {
                InitialDirectory = Directory.GetCurrentDirectory(),
                Multiselect = false,
                AddExtension = true,
                CheckFileExists = true
            };
            if (openFile.ShowDialog() == DialogResult.OK) result = openFile.FileName;
            return result;
        }

        /// <summary>
        /// Show a basic OpenFile Dialog, specifing a tile and/or a initial directory.
        /// </summary>
        /// <param name="title">The Title to display.</param>
        /// <param name="filter">The file filter to use as string.</param>
        /// <param name="initialDirectory">The initial directory to sue for this dialog.</param>
        /// <param name="selectMultiple">Used to determine that this is a multifile dialog.</param>
        /// <returns>The user input as string array or a empty string[].</returns>
        public static string[] ShowOpenFile(string title, string filter, string initialDirectory, bool selectMultiple) {
            string[] result = new string[0];
            OpenFileDialog openFile = new OpenFileDialog();
            if (title != string.Empty) openFile.Title = title;
            if (filter != string.Empty) openFile.Filter = filter;
            if (initialDirectory != string.Empty) openFile.InitialDirectory = initialDirectory;
            else openFile.InitialDirectory = Directory.GetCurrentDirectory();
            openFile.Multiselect = true;
            openFile.AddExtension = true;
            openFile.CheckFileExists = true;
            if (openFile.ShowDialog() == DialogResult.OK) {
                result = new string[openFile.FileNames.Length];
                result = openFile.FileNames;
            }
            return result;
        }

        /// <summary>
        /// Show a basic OpenFile Dialog, specifing a tile and/or a initial directory.
        /// </summary>
        /// <param name="title">The Title to display.</param>
        /// <param name="filter">The file filter to use as string.</param>
        /// <param name="name">The name for the file to display.</param>
        /// <param name="initialDirectory">The initial directory to sue for this dialog.</param>
        /// <param name="selectMultiple">Used to determine that this is a multifile dialog.</param>
        /// <returns>The user input as string array or a empty string[].</returns>
        public static string[] ShowOpenFile(string title, string filter, string name, string initialDirectory, bool selectMultiple) {
            string[] result = new string[0];
            OpenFileDialog openFile = new OpenFileDialog();
            if (title != string.Empty) openFile.Title = title;
            if (filter != string.Empty) openFile.Filter = filter;
            if (name != string.Empty) openFile.FileName = name;
            if (initialDirectory != string.Empty) openFile.InitialDirectory = initialDirectory;
            else openFile.InitialDirectory = Directory.GetCurrentDirectory();
            openFile.Multiselect = true;
            openFile.AddExtension = true;
            openFile.CheckFileExists = true;
            if (openFile.ShowDialog() == DialogResult.OK) {
                result = new string[openFile.FileNames.Length];
                result = openFile.FileNames;
            }
            return result;
        }

        /// <summary>
        /// Show a basic OpenFile Dialog, specifing a tile and/or a initial directory.
        /// </summary>
        /// <param name="selectMultiple">Used to determine that this is a multifile dialog.</param>
        /// <returns>The user input as string array or a empty string[].</returns>
        public static string[] ShowOpenFile(bool selectMultiple) {
            string[] result = new string[0];
            OpenFileDialog openFile = new OpenFileDialog() {
                InitialDirectory = Directory.GetCurrentDirectory(),
                Multiselect = true,
                AddExtension = true,
                CheckFileExists = true
            };
            if (openFile.ShowDialog() == DialogResult.OK) {
                result = new string[openFile.FileNames.Length];
                result = openFile.FileNames;
            }
            return result;
        }

        /// <summary>
        /// Show a basic SaveFile Dialog, speciging a title.
        /// </summary>
        /// <param name="title">The Title of the MessageBox Dialog.</param>
        /// <param name="filter">The file filter to use.</param>
        /// <param name="initialDirectory">The root directory to display.</param>
        /// <returns>The Dialog Result.</returns>
        public static string ShowSaveFile(string title, string filter, string initialDirectory) {
            string result = string.Empty;
            SaveFileDialog saveFile = new SaveFileDialog();
            if (title != string.Empty) saveFile.Title = title;
            if (initialDirectory != string.Empty) saveFile.InitialDirectory = initialDirectory;
            else saveFile.InitialDirectory = Directory.GetCurrentDirectory();
            if (filter != string.Empty) saveFile.Filter = "";
            saveFile.AddExtension = true;
            saveFile.OverwritePrompt = true;
            if (saveFile.ShowDialog() == DialogResult.OK) result = saveFile.FileName;
            return result;
        }

        /// <summary>
        /// Show a basic SaveFile Dialog.
        /// </summary>
        /// <returns>The Dialog Result.</returns>
        public static string ShowSaveFile() {
            string result = string.Empty;
            SaveFileDialog saveFile = new SaveFileDialog() {
                InitialDirectory = Directory.GetCurrentDirectory(),
                AddExtension = true,
                OverwritePrompt = true
            };
            if (saveFile.ShowDialog() == DialogResult.OK) result = saveFile.FileName;
            return result;
        }

        /// <summary>
        /// A simple FolderBrowseDialog, specifing a tile and/or a initial directory.
        /// </summary>
        /// <param name="title">The Title to display.</param>
        /// <param name="initialDirectory">The initial directory to sue for this dialog.</param>
        /// <returns>The user input as string or a empty string.</returns>
        public static string ShowFolderBrowse(string title, string initialDirectory) {
            string result = string.Empty;
            FolderBrowserDialog browseFolder = new FolderBrowserDialog() {
                ShowNewFolderButton = false,
                Description = title
            };
            if (initialDirectory != string.Empty) browseFolder.SelectedPath = initialDirectory;
            else browseFolder.SelectedPath = Directory.GetCurrentDirectory();
            if (browseFolder.ShowDialog() == DialogResult.OK) result = browseFolder.SelectedPath;
            return result;
        }

        /// <summary>
        /// A simple FolderBrowseDialog, specifing a tile.
        /// </summary>
        /// <returns>The user input as string or a empty string.</returns>
        public static string ShowFolderBrowse() {
            string result = string.Empty;
            FolderBrowserDialog browseFolder = new FolderBrowserDialog() {
                ShowNewFolderButton = false,
                SelectedPath = Directory.GetCurrentDirectory()
            };
            if (browseFolder.ShowDialog() == DialogResult.OK) result = browseFolder.SelectedPath;
            return result;
        }
        #endregion Functions
    }

    /// <summary>
    /// SwissKnife main class.
    /// </summary>
    class SwissKnife {
        private const string sh = @"shell\";
        private const string cmd = @"command\";
        private const string open = sh + @"open\" + cmd;

        /// <summary>
        /// Gets the default Application to run with for a specific extension.
        /// </summary>
        /// <param name="type">The type of for what we shall check for.</param>
        /// <returns>The path to the default file application or and empty string.</returns>
        public static string GetDefaultApp(string type) {
            RegistryKey defaultApp = Registry.ClassesRoot.OpenSubKey(type + open);
            if (defaultApp != null) return defaultApp.GetValue("").ToString().Replace("%1", "").Replace("\"", "");
            else return string.Empty;
        }
    }
}
