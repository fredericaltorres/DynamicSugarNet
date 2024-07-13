using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DynamicSugar;
using System.Dynamic;
using System.Reflection;
using System.Text.RegularExpressions;
using DynamicSugar;

namespace DynamicSugarSharp_UnitTests
{
    [TestClass]
    public class JsonExtractorTests
    {
        [TestMethod]
        public void ExtractOneObjectWithTextBeforeAndAfter()
        {
            var Json1 = @"TATA { ""a"": 1 } TUTU";
            var expectedJson1 = "{\r\n  \"a\": 1\r\n}";
            var text = $"{Json1}";
            var result =  JsonExtractor.Extract(text);
            Assert.AreEqual(expectedJson1, result);
        }

        [TestMethod]
        public void ExtractOneInvalidObject_BadCurlyBracketPosition ()
        {
            var Json1 = @"}   ""a""   :   1 {";
            var text = $"noiseStart {Json1} noiseEnd";
            var result = JsonExtractor.Extract(text);
            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void ExtractOneInvalidObject_JsonSyntaxError()
        {
            var Json1 = @"{   ""a""      1 }";
            var text = $"noiseStart {Json1} noiseEnd";
            var result = JsonExtractor.Extract(text);
            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void ExtractOneObject()
        {
            var Json1 = @"{   ""a""   :   1 }";
            var expectedJson1 = "{\r\n  \"a\": 1\r\n}";
            var text = $"noiseStart {Json1} noiseEnd";
            var result = JsonExtractor.Extract(text);
            Assert.AreEqual(expectedJson1, result);
        }

        [TestMethod]
        public void ExtractOneArray()
        {
            var Json1 = @"[1, 2, 3]";
            var expectedJson1 = "[\r\n  1,\r\n  2,\r\n  3\r\n]";
            var text = $"noiseStart {Json1} noiseEnd";
            var result = JsonExtractor.Extract(text);
            Assert.AreEqual(expectedJson1, result);
        }


        [TestMethod]
        public void ExtractOneArrayOfObject()
        {
            var Json1 = @"[{ ""a"":1 }, { ""b"":1 }]";
            var expectedJson1 = @"[
  {
    ""a"": 1
  },
  {
    ""b"": 1
  }
]";
            var text = $"noiseStart {Json1} noiseEnd";
            var result = JsonExtractor.Extract(text);
            Assert.AreEqual(expectedJson1, result);
        }


        [TestMethod]
        public void ExtractInvalidJson()
        {
            var Json1 = @"[1,2,3";
            var text = $"noiseStart {Json1} noiseEnd";
            var result = JsonExtractor.Extract(text);
            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void ExtractOneInvalidArrayOfInt()
        {
            var Json1 = @"]1,2,3[";
            var text = $"noiseStart {Json1} noiseEnd";
            var result = JsonExtractor.Extract(text);
            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void ExtractOneObjectWithTimeStampInSquareBraket()
        {
            var Json1 = @"[2021-12-10T00:00:20.257Z]  {""IsSuccessStatusCode"":true } ";
            var expectedJson1 = @"{
  ""IsSuccessStatusCode"": true
}";
            var text = Json1;
            var result = JsonExtractor.Extract(text);
            Assert.AreEqual(expectedJson1, result);
        }



        [TestMethod]
        public void ExtractOneObjectWithTimeStampInCurlyBraket()
        {
            var Json1 = @"{2021-12-10T00:00:20.257Z}  {""IsSuccessStatusCode"":true } ";
            var expectedJson1 = @"{
  ""IsSuccessStatusCode"": true
}";
            var text = Json1;
            var result = JsonExtractor.Extract(text);
            Assert.AreEqual(expectedJson1, result);
        }


        [TestMethod]
        public void Extract_DateInBracket_JsonBracketContainingOneObject()
        {
            var Json1 = @"[2024-07-12T10:23:50.403Z]  JSON Message [{""JobId"":14669442}]";
            var expectedJson1 = @"[
  {
    ""JobId"": 14669442
  }
]";
            var text = Json1;
            var result = JsonExtractor.Extract(text);
            Assert.AreEqual(expectedJson1, result);
        }


        [TestMethod]
        public void Extract_DateInBracket_JsonBracketContainingTwoObject()
        {
            var Json1 = @"[2024-07-12T10:23:50.403Z]  JSON Message [{""JobId"":1111}, {""JobId"":2222}]";
            var expectedJson1 = @"[
  {
    ""JobId"": 1111
  },
  {
    ""JobId"": 2222
  }
]";
            var text = Json1;
            var result = JsonExtractor.Extract(text);
            Assert.AreEqual(expectedJson1, result);
        }

        [TestMethod]
        public void Extract_DateInBracket_JsonObjectContainingArray()
        {
            var Json1 = @"[2024-07-12T10:23:40.325Z] | [HTTPCallStatus] Content:{""VideoId"":0, ""Timings"":[], ""mimeType"":null }";
            var expectedJson1 = @"{
  ""VideoId"": 0,
  ""Timings"": [],
  ""mimeType"": null
}";
            var text = Json1;
            var result = JsonExtractor.Extract(text);
            Assert.AreEqual(expectedJson1, result);
        }


        [TestMethod]
        public void Extract_DateInBracket_DoubleJsonObjectNested()
        {
            var Json1 = @"{ ""created"": ""2024-07-12T06:26:00.78"", ""author"": { ""authorFullName"": ""Frederic Torres"" } }";
            var expectedJson1 = @"{
  ""created"": ""2024-07-12T06:26:00.78"",
  ""author"": {
    ""authorFullName"": ""Frederic Torres""
  }
}";
            var text = Json1;
            var result = JsonExtractor.Extract(text);
            Assert.AreEqual(expectedJson1, result);
        }

        [TestMethod]
        public void Extract_DateInBracket_tRIPLLEJsonObjectNested()
        {
            var Json1 = @"{ ""created"": ""2024-07-12T06:26:00.78"", ""author"": { ""authorFullName"": ""Frederic Torres"", ""O"" : { ""zaza"" : true } } }";
            var expectedJson1 = @"{
  ""created"": ""2024-07-12T06:26:00.78"",
  ""author"": {
    ""authorFullName"": ""Frederic Torres"",
    ""O"": {
      ""zaza"": true
    }
  }
}";
            var text = Json1;
            var result = JsonExtractor.Extract(text);
            Assert.AreEqual(expectedJson1, result);
        }

        [TestMethod]
        public void Extract_ComplexCase()
        {
            var Json1 = @"[2024-07-13T01:32:16.508Z] | [HTTPCallStatus] Content:{""created"":""2024-07-12T21:31:46.463"",""authorID"":1157306,""author"":{""authorFullName"":""admin admin"",""bio"":"""",""title"":""Administrator"",""userImageID"":20090,""email"":""frederic.torres@bigtincan.com""},""authoringLock"":false,""duration"":9,""lastUpdated"":""2024-07-12T21:32:00.213"",""presentationId"":779512986,""presentationType"":""BB"",""slideVersionNumber"":4,""slides"":[{""animationTimings"":[],""animationCount"":0,""firstSequenceTimed"":true,""slideID"":10620960,""slideNumber"":1,""slideType"":""CHEETH"",""title"":""TITLE"",""hasAnimation"":true,""hasAttachment"":false,""hasAudio"":true,""hasNarration"":false,""hasNotes"":false,""hasClosedCaption"":false,""notesLength"":0,""fileSize"":-1,""ttsVoiceDefinition"":null,""duration"":4,""videoDuration"":0,""supplementID"":0,""mimeType"":null},{""animationTimings"":[],""animationCount"":0,""firstSequenceTimed"":false,""slideID"":10620961,""slideNumber"":2,""slideType"":""CHEETH"",""title"":""Who am i?"",""hasAnimation"":false,""hasAttachment"":false,""hasAudio"":false,""hasNarration"":false,""hasNotes"":false,""hasClosedCaption"":false,""notesLength"":0,""fileSize"":-1,""ttsVoiceDefinition"":null,""duration"":5,""videoDuration"":0,""supplementID"":0,""mimeType"":null}],""slideOrders"":[{""slideId"":10620960,""orderNumber"":1,""chapterTitle"":null,""slideTitle"":null},{""slideId"":10620961,""orderNumber"":2,""chapterTitle"":null,""slideTitle"":null}],""activeJobIds"":[],""canGenerateTextToSpeech"":false,""hasQuestions"":false,""canEdit"":null,""courseID"":0,""signalRUrl"":""https://fun-qaz1-signalrjobprogress-eastus.azurewebsites.net"",""companySettings"":{""canChangePresentationStatus"":true,""authorsMayOverrideHideContentFromSearch"":false,""allowAuthorOverrideAllowCommentsDefault"":true,""allowAuthorOverrideAllowRatingsDefault"":true,""allowAuthorsToCopyDefault"":true,""disableTags"":false,""canSetCompletionCriteria"":true,""allowAuthorOverrideDisplayCompletionIndicator"":true,""defaultCompletionCertificateId"":-1,""defaultCompletionCertificateMessage"":""Congratulations on Completing <<PresentationTitle>>"",""canSetRequireLogin"":true,""canSetPassword"":true,""allowAuthorOverrideAllowAuthorsToCopy"":true,""canSetAsWrap"":true,""canSetShortTitle"":true,""allowAuthorOverridePlayerTheme"":true,""canSetAspectRatio"":true,""allowTesting"":true,""authorCanSetSlideNotesConfig"":true,""allowEmailPresentation"":true,""allowAuthorOverrideDisplayEmbedInViewer"":true,""displayQAEmail_AllowAuthorsOverrideDefault"":true,""canSetPersonalizationSettings"":true,""authorCanSetQAConfig"":true,""allowAuthorOverrideEnableRememberMeOnGuestBook"":true,""allowAuthorOverrideGuestBookFormLabels"":false,""defaultPlayerThemeId"":13,""canEnableOfflineViewing"":false,""supportsOfflineViewing"":false,""allowInteractionRetries"":true,""allowAuthorOverrideLikert"":true,""defaultLikertHeadings"":[""Strongly Disagree"",""Disagree"",""Neutral"",""Agree"",""Strongly Agree"",""Very Strongly Agree"",""N/A""],""allowPrinting"":true,""allowPresentationDownload"":true,""allowBypassPlayer"":true,""enableFileScanning"":false,""allowTextToSpeech"":true,""enableAuthoringAIVoiceLibrary"":false,""enableCustomVoices"":false,""customVoiceSlotsAllowed"":0,""enableGenerateCaptionsAndNotes"":false,""enableTextTranslation"":false,""allowPodcast"":true},""status"":""Inactive"",""categoryID"":637089,""isHiddenInContentPortal"":false,""loginRequired"":true,""allowAuthorsToCopy"":true,""isWrap"":false,""shortTitle"":"""",""playerThemeId"":0,""aspectRatio"":""4x3"",""displayCompanyLogo"":true,""companyImageId"":0,""displayPhotoPlusBio"":true,""displayScore"":false,""displaySlideNotes"":true,""navRule"":""Default"",""manualAdvance"":false,""loopPresentation"":false,""randomQuestions"":false,""enablePlaybackSpeed"":false,""allowEmailPresentation"":false,""displayEmbedInViewer"":true,""displayQAEmail"":true,""emailQuestions"":"""",""isResumable"":true,""enforceNumberOfAttemptsPerQuestion"":false,""bypassPlayer"":false,""displayQASection"":true,""faqs"":null,""usePassword"":false,""password"":null,""expirationDate"":null,""expirationWarningEmailList"":"""",""emailViewerReceipts"":""Off"",""authorEmail"":"""",""allowComment"":true,""allowRate"":true,""enableOfflineViewing"":true,""customFilterDetailIds"":[],""tags"":[],""personalizationSettings"":null,""completionCriteria"":null,""guestbookSettings"":{""enableGuesbook"":false,""intro"":"""",""enableRememberMe"":true,""fields"":[{""name"":""dept"",""isVisible"":false,""isRequired"":false,""customLabel"":"""",""chooseFromList"":false,""listValues"":[]},{""name"":""title"",""isVisible"":false,""isRequired"":false,""customLabel"":"""",""chooseFromList"":false,""listValues"":[]},{""name"":""first_name"",""isVisible"":true,""isRequired"":false,""customLabel"":"""",""chooseFromList"":false,""listValues"":[]},{""name"":""last_name"",""isVisible"":true,""isRequired"":false,""customLabel"":"""",""chooseFromList"":false,""listValues"":[]},{""name"":""phone"",""isVisible"":false,""isRequired"":false,""customLabel"":"""",""chooseFromList"":false,""listValues"":[]},{""name"":""company"",""isVisible"":false,""isRequired"":false,""customLabel"":"""",""chooseFromList"":false,""listValues"":[]},{""name"":""email"",""isVisible"":true,""isRequired"":false,""customLabel"":"""",""chooseFromList"":false,""listValues"":[]}]},""allowPodcast"":false,""description"":"""",""title"":""2_Slides_WithAudio_Wav""}";
            var text = Json1;
            var result = JsonExtractor.Extract(text);
            Assert.AreEqual(LargeExpectedJson, result);
        }
    
const string LargeExpectedJson = @"{
  ""created"": ""2024-07-12T21:31:46.463"",
  ""authorID"": 1157306,
  ""author"": {
    ""authorFullName"": ""admin admin"",
    ""bio"": """",
    ""title"": ""Administrator"",
    ""userImageID"": 20090,
    ""email"": ""frederic.torres@bigtincan.com""
  },
  ""authoringLock"": false,
  ""duration"": 9,
  ""lastUpdated"": ""2024-07-12T21:32:00.213"",
  ""presentationId"": 779512986,
  ""presentationType"": ""BB"",
  ""slideVersionNumber"": 4,
  ""slides"": [
    {
      ""animationTimings"": [],
      ""animationCount"": 0,
      ""firstSequenceTimed"": true,
      ""slideID"": 10620960,
      ""slideNumber"": 1,
      ""slideType"": ""CHEETH"",
      ""title"": ""TITLE"",
      ""hasAnimation"": true,
      ""hasAttachment"": false,
      ""hasAudio"": true,
      ""hasNarration"": false,
      ""hasNotes"": false,
      ""hasClosedCaption"": false,
      ""notesLength"": 0,
      ""fileSize"": -1,
      ""ttsVoiceDefinition"": null,
      ""duration"": 4,
      ""videoDuration"": 0,
      ""supplementID"": 0,
      ""mimeType"": null
    },
    {
      ""animationTimings"": [],
      ""animationCount"": 0,
      ""firstSequenceTimed"": false,
      ""slideID"": 10620961,
      ""slideNumber"": 2,
      ""slideType"": ""CHEETH"",
      ""title"": ""Who am i?"",
      ""hasAnimation"": false,
      ""hasAttachment"": false,
      ""hasAudio"": false,
      ""hasNarration"": false,
      ""hasNotes"": false,
      ""hasClosedCaption"": false,
      ""notesLength"": 0,
      ""fileSize"": -1,
      ""ttsVoiceDefinition"": null,
      ""duration"": 5,
      ""videoDuration"": 0,
      ""supplementID"": 0,
      ""mimeType"": null
    }
  ],
  ""slideOrders"": [
    {
      ""slideId"": 10620960,
      ""orderNumber"": 1,
      ""chapterTitle"": null,
      ""slideTitle"": null
    },
    {
      ""slideId"": 10620961,
      ""orderNumber"": 2,
      ""chapterTitle"": null,
      ""slideTitle"": null
    }
  ],
  ""activeJobIds"": [],
  ""canGenerateTextToSpeech"": false,
  ""hasQuestions"": false,
  ""canEdit"": null,
  ""courseID"": 0,
  ""signalRUrl"": ""https://fun-qaz1-signalrjobprogress-eastus.azurewebsites.net"",
  ""companySettings"": {
    ""canChangePresentationStatus"": true,
    ""authorsMayOverrideHideContentFromSearch"": false,
    ""allowAuthorOverrideAllowCommentsDefault"": true,
    ""allowAuthorOverrideAllowRatingsDefault"": true,
    ""allowAuthorsToCopyDefault"": true,
    ""disableTags"": false,
    ""canSetCompletionCriteria"": true,
    ""allowAuthorOverrideDisplayCompletionIndicator"": true,
    ""defaultCompletionCertificateId"": -1,
    ""defaultCompletionCertificateMessage"": ""Congratulations on Completing <<PresentationTitle>>"",
    ""canSetRequireLogin"": true,
    ""canSetPassword"": true,
    ""allowAuthorOverrideAllowAuthorsToCopy"": true,
    ""canSetAsWrap"": true,
    ""canSetShortTitle"": true,
    ""allowAuthorOverridePlayerTheme"": true,
    ""canSetAspectRatio"": true,
    ""allowTesting"": true,
    ""authorCanSetSlideNotesConfig"": true,
    ""allowEmailPresentation"": true,
    ""allowAuthorOverrideDisplayEmbedInViewer"": true,
    ""displayQAEmail_AllowAuthorsOverrideDefault"": true,
    ""canSetPersonalizationSettings"": true,
    ""authorCanSetQAConfig"": true,
    ""allowAuthorOverrideEnableRememberMeOnGuestBook"": true,
    ""allowAuthorOverrideGuestBookFormLabels"": false,
    ""defaultPlayerThemeId"": 13,
    ""canEnableOfflineViewing"": false,
    ""supportsOfflineViewing"": false,
    ""allowInteractionRetries"": true,
    ""allowAuthorOverrideLikert"": true,
    ""defaultLikertHeadings"": [
      ""Strongly Disagree"",
      ""Disagree"",
      ""Neutral"",
      ""Agree"",
      ""Strongly Agree"",
      ""Very Strongly Agree"",
      ""N/A""
    ],
    ""allowPrinting"": true,
    ""allowPresentationDownload"": true,
    ""allowBypassPlayer"": true,
    ""enableFileScanning"": false,
    ""allowTextToSpeech"": true,
    ""enableAuthoringAIVoiceLibrary"": false,
    ""enableCustomVoices"": false,
    ""customVoiceSlotsAllowed"": 0,
    ""enableGenerateCaptionsAndNotes"": false,
    ""enableTextTranslation"": false,
    ""allowPodcast"": true
  },
  ""status"": ""Inactive"",
  ""categoryID"": 637089,
  ""isHiddenInContentPortal"": false,
  ""loginRequired"": true,
  ""allowAuthorsToCopy"": true,
  ""isWrap"": false,
  ""shortTitle"": """",
  ""playerThemeId"": 0,
  ""aspectRatio"": ""4x3"",
  ""displayCompanyLogo"": true,
  ""companyImageId"": 0,
  ""displayPhotoPlusBio"": true,
  ""displayScore"": false,
  ""displaySlideNotes"": true,
  ""navRule"": ""Default"",
  ""manualAdvance"": false,
  ""loopPresentation"": false,
  ""randomQuestions"": false,
  ""enablePlaybackSpeed"": false,
  ""allowEmailPresentation"": false,
  ""displayEmbedInViewer"": true,
  ""displayQAEmail"": true,
  ""emailQuestions"": """",
  ""isResumable"": true,
  ""enforceNumberOfAttemptsPerQuestion"": false,
  ""bypassPlayer"": false,
  ""displayQASection"": true,
  ""faqs"": null,
  ""usePassword"": false,
  ""password"": null,
  ""expirationDate"": null,
  ""expirationWarningEmailList"": """",
  ""emailViewerReceipts"": ""Off"",
  ""authorEmail"": """",
  ""allowComment"": true,
  ""allowRate"": true,
  ""enableOfflineViewing"": true,
  ""customFilterDetailIds"": [],
  ""tags"": [],
  ""personalizationSettings"": null,
  ""completionCriteria"": null,
  ""guestbookSettings"": {
    ""enableGuesbook"": false,
    ""intro"": """",
    ""enableRememberMe"": true,
    ""fields"": [
      {
        ""name"": ""dept"",
        ""isVisible"": false,
        ""isRequired"": false,
        ""customLabel"": """",
        ""chooseFromList"": false,
        ""listValues"": []
      },
      {
        ""name"": ""title"",
        ""isVisible"": false,
        ""isRequired"": false,
        ""customLabel"": """",
        ""chooseFromList"": false,
        ""listValues"": []
      },
      {
        ""name"": ""first_name"",
        ""isVisible"": true,
        ""isRequired"": false,
        ""customLabel"": """",
        ""chooseFromList"": false,
        ""listValues"": []
      },
      {
        ""name"": ""last_name"",
        ""isVisible"": true,
        ""isRequired"": false,
        ""customLabel"": """",
        ""chooseFromList"": false,
        ""listValues"": []
      },
      {
        ""name"": ""phone"",
        ""isVisible"": false,
        ""isRequired"": false,
        ""customLabel"": """",
        ""chooseFromList"": false,
        ""listValues"": []
      },
      {
        ""name"": ""company"",
        ""isVisible"": false,
        ""isRequired"": false,
        ""customLabel"": """",
        ""chooseFromList"": false,
        ""listValues"": []
      },
      {
        ""name"": ""email"",
        ""isVisible"": true,
        ""isRequired"": false,
        ""customLabel"": """",
        ""chooseFromList"": false,
        ""listValues"": []
      }
    ]
  },
  ""allowPodcast"": false,
  ""description"": """",
  ""title"": ""2_Slides_WithAudio_Wav""
}";

    }
}