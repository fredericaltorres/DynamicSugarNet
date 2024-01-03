using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Text.RegularExpressions;
#if !MONOTOUCH
using System.Dynamic;
#endif

namespace DynamicSugar
{
    /// <summary>
    /// Dynamic Sharp Exception
    /// </summary>
    public class AssertFailedException : System.Exception {

        public AssertFailedException(string message) : base(message) { }
    }
    /// <summary>
    /// Dynamic Sharp Helper Class
    /// </summary>
    public static partial class DS
    {

        public static class Assert
        {
            public static void IsTrue(bool exp, string message)
            {
                if (!exp)
                    throw new AssertFailedException($"Expected to be true: {message}");
            }

            public static void AreNotEqual<T>(List<T> l1, List<T> l2)
            {
                if (DS.ListHelper.Identical(l1, l2))
                    throw new AssertFailedException(String.Format("List are equal L1:'{0}', L2:'{1}'", DS.ListHelper.Format(l1), DS.ListHelper.Format(l2)));
            }

            /// <summary>
            /// Assert that 2 List Of T are equal
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="l1"></param>
            /// <param name="l2"></param>
            public static void AreEqual<T>(List<T> l1, List<T> l2)
            {

                if (!DS.ListHelper.Identical(l1, l2))
                    throw new AssertFailedException(String.Format("List are not equal L1:'{0}', L2:'{1}'", DS.ListHelper.Format(l1), DS.ListHelper.Format(l2)));
            }

            public static void AreEqualProperties(object poco, Dictionary<string, object> propertyNameValues)
            {

                if (poco is Dictionary<string, object>)
                {
                    var dic = poco as Dictionary<string, object>;
                    foreach (var k in dic)
                    {
                        var actualValue = k.Value;
                        if (actualValue == null && propertyNameValues[k.Key] == null)
                        {
                            // null == null
                        }
                        else if (!actualValue.Equals(propertyNameValues[k.Key]))
                            throw new AssertFailedException("AssertValueTypeProperties failed Property:{0}, Actual:{1}, Expected:{2}".FormatString(k.Key, actualValue, propertyNameValues[k.Key]));
                    }
                }
                else
                {
                    foreach (var k in propertyNameValues)
                    {
                        var actualValue = ReflectionHelper.GetProperty(poco, k.Key);
                        if (actualValue == null && propertyNameValues[k.Key] == null)
                        {
                            // null == null
                        }
                        else if (!actualValue.Equals(propertyNameValues[k.Key]))
                            throw new AssertFailedException("AssertValueTypeProperties failed Property:{0}, Actual:{1}, Expected:{2}".FormatString(k.Key, actualValue, propertyNameValues[k.Key]));
                    }
                }
            }

            public static void AreEqualProperties(object poco, object propertyNameValues)
            {

                if (propertyNameValues is Dictionary<string, object>)
                {
                    AreEqualProperties(poco, propertyNameValues as Dictionary<string, object>);
                }
                else
                {
                    AreEqualProperties(poco, DS.Dictionary(propertyNameValues));
                }
            }

            public static List<string> GetBalancedParenthesisExpression(List<string> expressionTokens, int currentIndex)
            {
                var r = new List<string>();
                var parenthesisCount = 0;
                while (true)
                {
                    var token = expressionTokens[currentIndex];
                    if (token == "(")
                    {
                        parenthesisCount++;
                        r.Add(token);
                    }
                    else if (token == ")")
                    {
                        parenthesisCount--;
                        r.Add(token);
                        if (parenthesisCount == 0)
                            break;
                    }
                    else
                    {
                        r.Add(token);
                    }
                    currentIndex++;
                }
                return r;
            }

            public static void Words(string text, string wordExpression)
            {
                var expressionTokens = Regex.Split(wordExpression, @"([*()\^\/]|(?<!E)[\ \&])").ToList();
                expressionTokens = expressionTokens.Filter(s => !String.IsNullOrEmpty(s.Trim())).ToList();
                Words(text, expressionTokens, 0);
            }

            public static void Words(string text, List<string> expressionTokens, int currentIndex)
            {
                while (true)
                {
                    var token = expressionTokens[currentIndex];
                    if (token == "(")
                    {
                        // The expression return start with the open and close parenthesis
                        var parenthesisExpression = GetBalancedParenthesisExpression(expressionTokens, currentIndex);
                        Words(text, parenthesisExpression, 1);
                        currentIndex += parenthesisExpression.Count;

                        if (currentIndex >= expressionTokens.Count) // Sometime needed
                            currentIndex = expressionTokens.Count - 1;

                        token = expressionTokens[currentIndex];
                        //if (token != ")")
                        //    throw new AssertFailedException($"Expected ')' at index {currentIndex} in expression '{String.Join("", expressionTokens)}'");
                    }
                    else if (token == ")")
                    {

                    }
                    else if (token == "&")
                    {
                         // Do nothing continue to evaluate the next token
                    }
                    else if (token == "|")
                    {
                        // The left token for the | passed, 
                        break; // we do not need to evaluate the right token
                    }
                    else
                    {
                        if (token == "regex")
                        {
                            currentIndex++;
                            var regExExp = expressionTokens[currentIndex];
                            var regex = new Regex(regExExp);
                            IsTrue(regex.IsMatch(text), $"Text:'{text}' regex '{regExExp}'");
                        }
                        else
                        {
                            IsTrue(text.Contains(token), $"Text:'{text}' contains '{token}'");
                        }
                    }
                    currentIndex++;
                    if(currentIndex == expressionTokens.Count)
                        break;
                }
            }
        }
    }
}
