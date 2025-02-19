﻿using System;
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
                if (poco is Dictionary<string, object>) // Assert A Dictionary
                {
                    var dic = poco as Dictionary<string, object>;
                    foreach (var k in dic)
                    {
                        var actualValue = k.Value;
                        var expectedValue = propertyNameValues[k.Key];
                        CompareExpectedAndActualValues(k.Key, actualValue, expectedValue);
                    }
                }
                else
                {
                    foreach (var k in propertyNameValues) // Assert a POCO
                    {
                        var actualValue = ReflectionHelper.GetProperty(poco, k.Key);
                        var expectedValue = propertyNameValues[k.Key];
                        CompareExpectedAndActualValues(k.Key, actualValue, expectedValue);
                    }
                }
            }

            private static void CompareExpectedAndActualValues(string key, object actualValue, object expectedValue)
            {
                if (expectedValue is Regex)
                {
                    var rx = expectedValue as Regex;
                    var r = rx.IsMatch(actualValue.ToString());
                    if (!r)
                        throw new AssertFailedException($"AssertValueTypeProperties failed Property:{key}, Actual:{actualValue}, Expected:{expectedValue}");
                }
                else
                {
                    if (actualValue == null && expectedValue == null)
                    {
                        // null == null
                    }
                    else if (!actualValue.Equals(expectedValue))
                        throw new AssertFailedException($"AssertValueTypeProperties failed Property:{key}, Actual:{actualValue}, Expected:{expectedValue}");
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

            /// <summary>
            /// 
            /// </summary>
            /// <param name="text">The text to validate</param>
            /// <param name="expressionTokens">The expression defining the validation "( aaa & bbb ) | ( ccc)" </param>
            public static void Words(string text, string wordExpression, int expectedMinimumCountMatch = -1)
            {
                var expressionTokens = Regex.Split(wordExpression, @"([*()\^\/]|(?<!E)[\ \&])").ToList();
                expressionTokens = expressionTokens.Filter(s => !String.IsNullOrEmpty(s.Trim())).ToList();
                var matchCount = Words(text, expressionTokens, 0, throwException: expectedMinimumCountMatch == -1, expectedMinimumCountMatch: expectedMinimumCountMatch);

                if (expectedMinimumCountMatch != -1)
                    IsTrue(matchCount >= expectedMinimumCountMatch, $"Text:'{text}' expression '{wordExpression}', matchCount:{matchCount}, expectedMinimumCountMatch:{expectedMinimumCountMatch}");
            }


            private static string GetNextToken(List<string> expressionTokens, int currentIndex)
            {
                if(currentIndex + 1 < expressionTokens.Count)
                    return expressionTokens[currentIndex+1];
                return null;
            }

            private static int Words(string text, List<string> expressionTokens, int currentIndex, bool throwException, int expectedMinimumCountMatch)
            {
                var countMatchMode = expectedMinimumCountMatch != -1;
                var r = 0;

                var previousEvaluation = false;
                while (true)
                {
                    var token = expressionTokens[currentIndex];
                    if (token == "(")
                    {
                        // The expression return start with the open and close parenthesis
                        var parenthesisExpression = GetBalancedParenthesisExpression(expressionTokens, currentIndex);
                        r += Words(text, parenthesisExpression, 1, throwException: throwException, expectedMinimumCountMatch);
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
                        if (!countMatchMode && previousEvaluation)
                        {
                            // When we are not in count match mode.
                            // But eval mode. we short-circuit the evaluation
                            break;
                        }
                    }
                    else
                    {
                        if (token == "regex")
                        {
                            currentIndex++;
                            var regExExp = expressionTokens[currentIndex];
                            var regex = new Regex(regExExp);
                            var exp = regex.IsMatch(text);
                            if (throwException)
                            {
                                IsTrue(exp, $"Text:'{text}' regex '{regExExp}'");
                            }
                            else
                            {
                                if (exp) r++; // We just count the match
                            }
                        }
                        else
                        {
                            var exp = text.Contains(token);
                            previousEvaluation = exp;
                            if (exp == false && GetNextToken(expressionTokens, currentIndex) == "|")
                            {
                                // Move to the next token hopefully one will match OR expression
                            }
                            else if (throwException)
                            {
                                IsTrue(exp, $"Text:'{text}' contains '{token}'");
                            }
                            else
                            {
                                if (exp) r++; // We just count the match
                            }
                        }
                    }
                    currentIndex++;
                    if(currentIndex == expressionTokens.Count)
                        break;
                }
                return r;
            }
        }
    }
}
