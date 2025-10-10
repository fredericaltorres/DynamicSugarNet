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
        public void ExtractComplexOnee()
        {
            var Json1 = @"2025/03/22 11:53:19.880 AM |                   [HTTPCallStatus] Content:[{""id"":""08dd688b-cdd0-4b59-83fb-65092684c557"",""name"":""FreddyPowerShell"",""characters"":[{""id"":""84fdf1e1-cecd-4eb1-a7f7-5f34f442985f"",""characterType"":""image"",""name"":""FredImage1"",""defaultVoice"":null,""gender"":""genderless"",""previewImageUrl"":""https://genie-ai-ap-southeast-2-419909608512.s3.ap-southeast-2.amazonaws.com/urn%3Abtc0%3Aidentity.bigtincan.org%3ACustomer%3A08da1db3-6ef1-4190-875a-e5713088e84d/avatars/characters/84fdf1e1-cecd-4eb1-a7f7-5f34f442985f/previewImage.jpg?X-Amz-Expires=21600&X-Amz-Security-Token=IQoJb3JpZ2luX2VjEGgaDmFwLXNvdXRoZWFzdC0yIkcwRQIhAJwEzqn7xb0OabkP57Bq90wRpG5A3IhmNfpjPp%2B453GKAiAXLt8MzJ1uPEy3fFC3hT1ZNXJsKTGq6j5Df%2FAyn1kyciqfBQjB%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F8BEAUaDDQxOTkwOTYwODUxMiIMZ%2BzOje5GTy%2F5nz2HKvMEcl8a8GIM47jUmxRN8oUeincmy1rxoMR%2FbxwNNqwoRJ1tg1yIpjCeoMU%2FiCM1AQ9DG7zYjND3M7XMbbDLKXhbHE8YgaV2gmTxh%2BYXYCfub5osH1nDDiLl8ijppZ9hFQ2dLkTT8tXsmQcvk%2BRBN7QaGCwV89Tvk%2BRSlGV8KQdDTB0phnVoV2965Mrf6x6xxy7ZWYsZdGjJuMnVqcadHKiWk0Zo6AxrzzvfyFt%2BXWtNLzdv5EYOt1x1%2BXYYgevtbc2zk5IiU21zM13PAjdHxLHDs6N5U9040fxF11BMmhwqBLgj4pwduTQKjKm5WE5zkIBFqQwk4as5O9ZimUAwehq0MCabSGBjOPbJqZdiUWyUlbbUVePp33%2BL%2FaDmspsjvB7I2lt128jGia%2FTp8l5bbadRa2FYYDXliq%2BxlnnKw2soGQ9JoYRxCLhCttDbJlPu4vcLHJkiRWCxUcKVouJo5h5VI08OZCKEDip7a0gCqO65Ll%2FXJhFwI65fOuwzB24lgK2Wwyrciuuy3V8Ct7PBMgy%2F%2BRr5FHDiYQ9I1Kwqq03ImVIqkoSlYR7oyxoSgxCjH4%2BHWx3E3CwhiejmK8rRW0RRyRd%2FGfXInVOhNq%2Ft%2B9bFvreCibmRPV%2Fmv336NFG6yFH5rao0oeva62BUe0FYvRcQIejshKsrV96FS9u889RXY5hoxKPltCdg9FafIQoGE%2F6xBl0QT5K6RtcvLCVxRt0TZQ4vTSBBj7Pb1VsNLHwVS4zgmdefvDBhk%2FshdPGbJAZuGfCAm5vAEkKiDejc32eDJovgQFwgH9w2Ci%2BG5rS8SdUAuKRO3ZhfjJG0qPDQzw5jocWMKqy%2B74GOpoB6fH8x2x3zK8HwG0Fu5T2CRCVbLvLbhr3O4ByGDfKjzpXCXvfFedKEOKkflbddJoF%2FH4Fiw%2BnrZzU%2BegN%2FopoQ7g%2BWOVsIOYnZLAqM3gseanY9FRvUtgRYIkFfWQyjEXyqDbiz0ZLOQHS6pVXYCfhK3nfWT%2BWWS6my56X3R7P3c4LmbUHdwn%2BsVVZIkGxVYS7vVn7eYDShjvxxw%3D%3D&response-cache-control=max-age%3D21600&X-Amz-Algorithm=AWS4-HMAC-SHA256&X-Amz-Credential=ASIAWDREQERAA4ROHKFF%2F20250322%2Fap-southeast-2%2Fs3%2Faws4_request&X-Amz-Date=20250322T155318Z&X-Amz-SignedHeaders=host&X-Amz-Signature=86941fb9cb47fbd49e69555a0342cc0b406be8688893ac95dc2e60807dbad7fc"",""previewVideoUrl"":null,""accessLevel"":""user"",""status"":""pending"",""isActive"":false}]},{""id"":""08dd68b5-8a7d-412c-8896-3851ba7e4594"",""name"":""Fred"",""characters"":[{""id"":""464be85d-2566-4f42-aaaf-41c4da375ffe"",""characterType"":""image"",""name"":""BSHIT.PLATFORM.UnitTest.Character"",""defaultVoice"":null,""gender"":""genderless"",""previewImageUrl"":""https://genie-ai-ap-southeast-2-419909608512.s3.ap-southeast-2.amazonaws.com/urn%3Abtc0%3Aidentity.bigtincan.org%3ACustomer%3A08da1db3-6ef1-4190-875a-e5713088e84d/avatars/characters/464be85d-2566-4f42-aaaf-41c4da375ffe/previewImage.jpg?X-Amz-Expires=21600&X-Amz-Security-Token=IQoJb3JpZ2luX2VjEGgaDmFwLXNvdXRoZWFzdC0yIkcwRQIhAJwEzqn7xb0OabkP57Bq90wRpG5A3IhmNfpjPp%2B453GKAiAXLt8MzJ1uPEy3fFC3hT1ZNXJsKTGq6j5Df%2FAyn1kyciqfBQjB%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F8BEAUaDDQxOTkwOTYwODUxMiIMZ%2BzOje5GTy%2F5nz2HKvMEcl8a8GIM47jUmxRN8oUeincmy1rxoMR%2FbxwNNqwoRJ1tg1yIpjCeoMU%2FiCM1AQ9DG7zYjND3M7XMbbDLKXhbHE8YgaV2gmTxh%2BYXYCfub5osH1nDDiLl8ijppZ9hFQ2dLkTT8tXsmQcvk%2BRBN7QaGCwV89Tvk%2BRSlGV8KQdDTB0phnVoV2965Mrf6x6xxy7ZWYsZdGjJuMnVqcadHKiWk0Zo6AxrzzvfyFt%2BXWtNLzdv5EYOt1x1%2BXYYgevtbc2zk5IiU21zM13PAjdHxLHDs6N5U9040fxF11BMmhwqBLgj4pwduTQKjKm5WE5zkIBFqQwk4as5O9ZimUAwehq0MCabSGBjOPbJqZdiUWyUlbbUVePp33%2BL%2FaDmspsjvB7I2lt128jGia%2FTp8l5bbadRa2FYYDXliq%2BxlnnKw2soGQ9JoYRxCLhCttDbJlPu4vcLHJkiRWCxUcKVouJo5h5VI08OZCKEDip7a0gCqO65Ll%2FXJhFwI65fOuwzB24lgK2Wwyrciuuy3V8Ct7PBMgy%2F%2BRr5FHDiYQ9I1Kwqq03ImVIqkoSlYR7oyxoSgxCjH4%2BHWx3E3CwhiejmK8rRW0RRyRd%2FGfXInVOhNq%2Ft%2B9bFvreCibmRPV%2Fmv336NFG6yFH5rao0oeva62BUe0FYvRcQIejshKsrV96FS9u889RXY5hoxKPltCdg9FafIQoGE%2F6xBl0QT5K6RtcvLCVxRt0TZQ4vTSBBj7Pb1VsNLHwVS4zgmdefvDBhk%2FshdPGbJAZuGfCAm5vAEkKiDejc32eDJovgQFwgH9w2Ci%2BG5rS8SdUAuKRO3ZhfjJG0qPDQzw5jocWMKqy%2B74GOpoB6fH8x2x3zK8HwG0Fu5T2CRCVbLvLbhr3O4ByGDfKjzpXCXvfFedKEOKkflbddJoF%2FH4Fiw%2BnrZzU%2BegN%2FopoQ7g%2BWOVsIOYnZLAqM3gseanY9FRvUtgRYIkFfWQyjEXyqDbiz0ZLOQHS6pVXYCfhK3nfWT%2BWWS6my56X3R7P3c4LmbUHdwn%2BsVVZIkGxVYS7vVn7eYDShjvxxw%3D%3D&response-cache-control=max-age%3D21600&X-Amz-Algorithm=AWS4-HMAC-SHA256&X-Amz-Credential=ASIAWDREQERAA4ROHKFF%2F20250322%2Fap-southeast-2%2Fs3%2Faws4_request&X-Amz-Date=20250322T155318Z&X-Amz-SignedHeaders=host&X-Amz-Signature=3de91a9bcbc4252805add9175d0258599956d9328b7d3b7df3301bb9339d0652"",""previewVideoUrl"":null,""accessLevel"":""user"",""status"":""active"",""isActive"":true},{""id"":""c4232916-b133-4157-8a91-574f49a4ef6b"",""characterType"":""image"",""name"":""BSHIT.PLATFORM.UnitTest.Character"",""defaultVoice"":null,""gender"":""genderless"",""previewImageUrl"":""https://genie-ai-ap-southeast-2-419909608512.s3.ap-southeast-2.amazonaws.com/urn%3Abtc0%3Aidentity.bigtincan.org%3ACustomer%3A08da1db3-6ef1-4190-875a-e5713088e84d/avatars/characters/c4232916-b133-4157-8a91-574f49a4ef6b/previewImage.jpg?X-Amz-Expires=21600&X-Amz-Security-Token=IQoJb3JpZ2luX2VjEGgaDmFwLXNvdXRoZWFzdC0yIkcwRQIhAJwEzqn7xb0OabkP57Bq90wRpG5A3IhmNfpjPp%2B453GKAiAXLt8MzJ1uPEy3fFC3hT1ZNXJsKTGq6j5Df%2FAyn1kyciqfBQjB%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F8BEAUaDDQxOTkwOTYwODUxMiIMZ%2BzOje5GTy%2F5nz2HKvMEcl8a8GIM47jUmxRN8oUeincmy1rxoMR%2FbxwNNqwoRJ1tg1yIpjCeoMU%2FiCM1AQ9DG7zYjND3M7XMbbDLKXhbHE8YgaV2gmTxh%2BYXYCfub5osH1nDDiLl8ijppZ9hFQ2dLkTT8tXsmQcvk%2BRBN7QaGCwV89Tvk%2BRSlGV8KQdDTB0phnVoV2965Mrf6x6xxy7ZWYsZdGjJuMnVqcadHKiWk0Zo6AxrzzvfyFt%2BXWtNLzdv5EYOt1x1%2BXYYgevtbc2zk5IiU21zM13PAjdHxLHDs6N5U9040fxF11BMmhwqBLgj4pwduTQKjKm5WE5zkIBFqQwk4as5O9ZimUAwehq0MCabSGBjOPbJqZdiUWyUlbbUVePp33%2BL%2FaDmspsjvB7I2lt128jGia%2FTp8l5bbadRa2FYYDXliq%2BxlnnKw2soGQ9JoYRxCLhCttDbJlPu4vcLHJkiRWCxUcKVouJo5h5VI08OZCKEDip7a0gCqO65Ll%2FXJhFwI65fOuwzB24lgK2Wwyrciuuy3V8Ct7PBMgy%2F%2BRr5FHDiYQ9I1Kwqq03ImVIqkoSlYR7oyxoSgxCjH4%2BHWx3E3CwhiejmK8rRW0RRyRd%2FGfXInVOhNq%2Ft%2B9bFvreCibmRPV%2Fmv336NFG6yFH5rao0oeva62BUe0FYvRcQIejshKsrV96FS9u889RXY5hoxKPltCdg9FafIQoGE%2F6xBl0QT5K6RtcvLCVxRt0TZQ4vTSBBj7Pb1VsNLHwVS4zgmdefvDBhk%2FshdPGbJAZuGfCAm5vAEkKiDejc32eDJovgQFwgH9w2Ci%2BG5rS8SdUAuKRO3ZhfjJG0qPDQzw5jocWMKqy%2B74GOpoB6fH8x2x3zK8HwG0Fu5T2CRCVbLvLbhr3O4ByGDfKjzpXCXvfFedKEOKkflbddJoF%2FH4Fiw%2BnrZzU%2BegN%2FopoQ7g%2BWOVsIOYnZLAqM3gseanY9FRvUtgRYIkFfWQyjEXyqDbiz0ZLOQHS6pVXYCfhK3nfWT%2BWWS6my56X3R7P3c4LmbUHdwn%2BsVVZIkGxVYS7vVn7eYDShjvxxw%3D%3D&response-cache-control=max-age%3D21600&X-Amz-Algorithm=AWS4-HMAC-SHA256&X-Amz-Credential=ASIAWDREQERAA4ROHKFF%2F20250322%2Fap-southeast-2%2Fs3%2Faws4_request&X-Amz-Date=20250322T155318Z&X-Amz-SignedHeaders=host&X-Amz-Signature=dda8ab237cc31694da691e87314d4e27eeb7d04aaf7c59622224daf46cd70a94"",""previewVideoUrl"":null,""accessLevel"":""user"",""status"":""active"",""isActive"":true},{""id"":""e0f27e1d-5893-4c45-99a8-ad8ae0fdd27f"",""characterType"":""image"",""name"":""Fred.04"",""defaultVoice"":null,""gender"":""genderless"",""previewImageUrl"":""https://genie-ai-ap-southeast-2-419909608512.s3.ap-southeast-2.amazonaws.com/urn%3Abtc0%3Aidentity.bigtincan.org%3ACustomer%3A08da1db3-6ef1-4190-875a-e5713088e84d/avatars/characters/e0f27e1d-5893-4c45-99a8-ad8ae0fdd27f/previewImage.jpg?X-Amz-Expires=21600&X-Amz-Security-Token=IQoJb3JpZ2luX2VjEGgaDmFwLXNvdXRoZWFzdC0yIkcwRQIhAJwEzqn7xb0OabkP57Bq90wRpG5A3IhmNfpjPp%2B453GKAiAXLt8MzJ1uPEy3fFC3hT1ZNXJsKTGq6j5Df%2FAyn1kyciqfBQjB%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F8BEAUaDDQxOTkwOTYwODUxMiIMZ%2BzOje5GTy%2F5nz2HKvMEcl8a8GIM47jUmxRN8oUeincmy1rxoMR%2FbxwNNqwoRJ1tg1yIpjCeoMU%2FiCM1AQ9DG7zYjND3M7XMbbDLKXhbHE8YgaV2gmTxh%2BYXYCfub5osH1nDDiLl8ijppZ9hFQ2dLkTT8tXsmQcvk%2BRBN7QaGCwV89Tvk%2BRSlGV8KQdDTB0phnVoV2965Mrf6x6xxy7ZWYsZdGjJuMnVqcadHKiWk0Zo6AxrzzvfyFt%2BXWtNLzdv5EYOt1x1%2BXYYgevtbc2zk5IiU21zM13PAjdHxLHDs6N5U9040fxF11BMmhwqBLgj4pwduTQKjKm5WE5zkIBFqQwk4as5O9ZimUAwehq0MCabSGBjOPbJqZdiUWyUlbbUVePp33%2BL%2FaDmspsjvB7I2lt128jGia%2FTp8l5bbadRa2FYYDXliq%2BxlnnKw2soGQ9JoYRxCLhCttDbJlPu4vcLHJkiRWCxUcKVouJo5h5VI08OZCKEDip7a0gCqO65Ll%2FXJhFwI65fOuwzB24lgK2Wwyrciuuy3V8Ct7PBMgy%2F%2BRr5FHDiYQ9I1Kwqq03ImVIqkoSlYR7oyxoSgxCjH4%2BHWx3E3CwhiejmK8rRW0RRyRd%2FGfXInVOhNq%2Ft%2B9bFvreCibmRPV%2Fmv336NFG6yFH5rao0oeva62BUe0FYvRcQIejshKsrV96FS9u889RXY5hoxKPltCdg9FafIQoGE%2F6xBl0QT5K6RtcvLCVxRt0TZQ4vTSBBj7Pb1VsNLHwVS4zgmdefvDBhk%2FshdPGbJAZuGfCAm5vAEkKiDejc32eDJovgQFwgH9w2Ci%2BG5rS8SdUAuKRO3ZhfjJG0qPDQzw5jocWMKqy%2B74GOpoB6fH8x2x3zK8HwG0Fu5T2CRCVbLvLbhr3O4ByGDfKjzpXCXvfFedKEOKkflbddJoF%2FH4Fiw%2BnrZzU%2BegN%2FopoQ7g%2BWOVsIOYnZLAqM3gseanY9FRvUtgRYIkFfWQyjEXyqDbiz0ZLOQHS6pVXYCfhK3nfWT%2BWWS6my56X3R7P3c4LmbUHdwn%2BsVVZIkGxVYS7vVn7eYDShjvxxw%3D%3D&response-cache-control=max-age%3D21600&X-Amz-Algorithm=AWS4-HMAC-SHA256&X-Amz-Credential=ASIAWDREQERAA4ROHKFF%2F20250322%2Fap-southeast-2%2Fs3%2Faws4_request&X-Amz-Date=20250322T155318Z&X-Amz-SignedHeaders=host&X-Amz-Signature=7d6afe33bd60146095bb0cfc8a8092537a3b78fc5adcfa88b37ddf8865bcc920"",""previewVideoUrl"":null,""accessLevel"":""user"",""status"":""pending"",""isActive"":false}]},{""id"":""08dd68b8-8ad9-4b25-8214-b076b85e4520"",""name"":""AutoTestGroup_226443875"",""characters"":[{""id"":""77bd96f8-a785-41fd-a174-a2481f4d35d1"",""characterType"":""image"",""name"":""Fred.04"",""defaultVoice"":null,""gender"":""genderless"",""previewImageUrl"":""https://genie-ai-ap-southeast-2-419909608512.s3.ap-southeast-2.amazonaws.com/urn%3Abtc0%3Aidentity.bigtincan.org%3ACustomer%3A08da1db3-6ef1-4190-875a-e5713088e84d/avatars/characters/77bd96f8-a785-41fd-a174-a2481f4d35d1/previewImage.jpg?X-Amz-Expires=21600&X-Amz-Security-Token=IQoJb3JpZ2luX2VjEGgaDmFwLXNvdXRoZWFzdC0yIkcwRQIhAJwEzqn7xb0OabkP57Bq90wRpG5A3IhmNfpjPp%2B453GKAiAXLt8MzJ1uPEy3fFC3hT1ZNXJsKTGq6j5Df%2FAyn1kyciqfBQjB%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F8BEAUaDDQxOTkwOTYwODUxMiIMZ%2BzOje5GTy%2F5nz2HKvMEcl8a8GIM47jUmxRN8oUeincmy1rxoMR%2FbxwNNqwoRJ1tg1yIpjCeoMU%2FiCM1AQ9DG7zYjND3M7XMbbDLKXhbHE8YgaV2gmTxh%2BYXYCfub5osH1nDDiLl8ijppZ9hFQ2dLkTT8tXsmQcvk%2BRBN7QaGCwV89Tvk%2BRSlGV8KQdDTB0phnVoV2965Mrf6x6xxy7ZWYsZdGjJuMnVqcadHKiWk0Zo6AxrzzvfyFt%2BXWtNLzdv5EYOt1x1%2BXYYgevtbc2zk5IiU21zM13PAjdHxLHDs6N5U9040fxF11BMmhwqBLgj4pwduTQKjKm5WE5zkIBFqQwk4as5O9ZimUAwehq0MCabSGBjOPbJqZdiUWyUlbbUVePp33%2BL%2FaDmspsjvB7I2lt128jGia%2FTp8l5bbadRa2FYYDXliq%2BxlnnKw2soGQ9JoYRxCLhCttDbJlPu4vcLHJkiRWCxUcKVouJo5h5VI08OZCKEDip7a0gCqO65Ll%2FXJhFwI65fOuwzB24lgK2Wwyrciuuy3V8Ct7PBMgy%2F%2BRr5FHDiYQ9I1Kwqq03ImVIqkoSlYR7oyxoSgxCjH4%2BHWx3E3CwhiejmK8rRW0RRyRd%2FGfXInVOhNq%2Ft%2B9bFvreCibmRPV%2Fmv336NFG6yFH5rao0oeva62BUe0FYvRcQIejshKsrV96FS9u889RXY5hoxKPltCdg9FafIQoGE%2F6xBl0QT5K6RtcvLCVxRt0TZQ4vTSBBj7Pb1VsNLHwVS4zgmdefvDBhk%2FshdPGbJAZuGfCAm5vAEkKiDejc32eDJovgQFwgH9w2Ci%2BG5rS8SdUAuKRO3ZhfjJG0qPDQzw5jocWMKqy%2B74GOpoB6fH8x2x3zK8HwG0Fu5T2CRCVbLvLbhr3O4ByGDfKjzpXCXvfFedKEOKkflbddJoF%2FH4Fiw%2BnrZzU%2BegN%2FopoQ7g%2BWOVsIOYnZLAqM3gseanY9FRvUtgRYIkFfWQyjEXyqDbiz0ZLOQHS6pVXYCfhK3nfWT%2BWWS6my56X3R7P3c4LmbUHdwn%2BsVVZIkGxVYS7vVn7eYDShjvxxw%3D%3D&response-cache-control=max-age%3D21600&X-Amz-Algorithm=AWS4-HMAC-SHA256&X-Amz-Credential=ASIAWDREQERAA4ROHKFF%2F20250322%2Fap-southeast-2%2Fs3%2Faws4_request&X-Amz-Date=20250322T155318Z&X-Amz-SignedHeaders=host&X-Amz-Signature=fd9e384cd3e94c2363717cd2ba344f7cf87eac1c6154cd37715ed7ebeaefba5d"",""previewVideoUrl"":null,""accessLevel"":""user"",""status"":""pending"",""isActive"":false}]},{""id"":""08dd68e0-be75-4cd6-85f8-35b8847cff81"",""name"":""AutoTestGroup_243719609"",""characters"":[{""id"":""73fbbb98-12d8-4d8e-8890-f6608094e2e3"",""characterType"":""image"",""name"":""Fred.04"",""defaultVoice"":null,""gender"":""genderless"",""previewImageUrl"":""https://genie-ai-ap-southeast-2-419909608512.s3.ap-southeast-2.amazonaws.com/urn%3Abtc0%3Aidentity.bigtincan.org%3ACustomer%3A08da1db3-6ef1-4190-875a-e5713088e84d/avatars/characters/73fbbb98-12d8-4d8e-8890-f6608094e2e3/previewImage.jpg?X-Amz-Expires=21600&X-Amz-Security-Token=IQoJb3JpZ2luX2VjEGgaDmFwLXNvdXRoZWFzdC0yIkcwRQIhAJwEzqn7xb0OabkP57Bq90wRpG5A3IhmNfpjPp%2B453GKAiAXLt8MzJ1uPEy3fFC3hT1ZNXJsKTGq6j5Df%2FAyn1kyciqfBQjB%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F8BEAUaDDQxOTkwOTYwODUxMiIMZ%2BzOje5GTy%2F5nz2HKvMEcl8a8GIM47jUmxRN8oUeincmy1rxoMR%2FbxwNNqwoRJ1tg1yIpjCeoMU%2FiCM1AQ9DG7zYjND3M7XMbbDLKXhbHE8YgaV2gmTxh%2BYXYCfub5osH1nDDiLl8ijppZ9hFQ2dLkTT8tXsmQcvk%2BRBN7QaGCwV89Tvk%2BRSlGV8KQdDTB0phnVoV2965Mrf6x6xxy7ZWYsZdGjJuMnVqcadHKiWk0Zo6AxrzzvfyFt%2BXWtNLzdv5EYOt1x1%2BXYYgevtbc2zk5IiU21zM13PAjdHxLHDs6N5U9040fxF11BMmhwqBLgj4pwduTQKjKm5WE5zkIBFqQwk4as5O9ZimUAwehq0MCabSGBjOPbJqZdiUWyUlbbUVePp33%2BL%2FaDmspsjvB7I2lt128jGia%2FTp8l5bbadRa2FYYDXliq%2BxlnnKw2soGQ9JoYRxCLhCttDbJlPu4vcLHJkiRWCxUcKVouJo5h5VI08OZCKEDip7a0gCqO65Ll%2FXJhFwI65fOuwzB24lgK2Wwyrciuuy3V8Ct7PBMgy%2F%2BRr5FHDiYQ9I1Kwqq03ImVIqkoSlYR7oyxoSgxCjH4%2BHWx3E3CwhiejmK8rRW0RRyRd%2FGfXInVOhNq%2Ft%2B9bFvreCibmRPV%2Fmv336NFG6yFH5rao0oeva62BUe0FYvRcQIejshKsrV96FS9u889RXY5hoxKPltCdg9FafIQoGE%2F6xBl0QT5K6RtcvLCVxRt0TZQ4vTSBBj7Pb1VsNLHwVS4zgmdefvDBhk%2FshdPGbJAZuGfCAm5vAEkKiDejc32eDJovgQFwgH9w2Ci%2BG5rS8SdUAuKRO3ZhfjJG0qPDQzw5jocWMKqy%2B74GOpoB6fH8x2x3zK8HwG0Fu5T2CRCVbLvLbhr3O4ByGDfKjzpXCXvfFedKEOKkflbddJoF%2FH4Fiw%2BnrZzU%2BegN%2FopoQ7g%2BWOVsIOYnZLAqM3gseanY9FRvUtgRYIkFfWQyjEXyqDbiz0ZLOQHS6pVXYCfhK3nfWT%2BWWS6my56X3R7P3c4LmbUHdwn%2BsVVZIkGxVYS7vVn7eYDShjvxxw%3D%3D&response-cache-control=max-age%3D21600&X-Amz-Algorithm=AWS4-HMAC-SHA256&X-Amz-Credential=ASIAWDREQERAA4ROHKFF%2F20250322%2Fap-southeast-2%2Fs3%2Faws4_request&X-Amz-Date=20250322T155318Z&X-Amz-SignedHeaders=host&X-Amz-Signature=470cf32215c09ed59bf5187b7b24b511a0831e2b0f9917056de749ea0d00b30d"",""previewVideoUrl"":null,""accessLevel"":""user"",""status"":""active"",""isActive"":true}]},{""id"":""08dd6956-956c-495d-8233-fb68f07d9aa9"",""name"":""AutoTestGroup_294333812"",""characters"":[{""id"":""81cadafb-08e4-45d3-a2cd-0f72f9542bc4"",""characterType"":""image"",""name"":""Fred.04"",""defaultVoice"":null,""gender"":""genderless"",""previewImageUrl"":""https://genie-ai-ap-southeast-2-419909608512.s3.ap-southeast-2.amazonaws.com/urn%3Abtc0%3Aidentity.bigtincan.org%3ACustomer%3A08da1db3-6ef1-4190-875a-e5713088e84d/avatars/characters/81cadafb-08e4-45d3-a2cd-0f72f9542bc4/previewImage.jpg?X-Amz-Expires=21600&X-Amz-Security-Token=IQoJb3JpZ2luX2VjEGgaDmFwLXNvdXRoZWFzdC0yIkcwRQIhAJwEzqn7xb0OabkP57Bq90wRpG5A3IhmNfpjPp%2B453GKAiAXLt8MzJ1uPEy3fFC3hT1ZNXJsKTGq6j5Df%2FAyn1kyciqfBQjB%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F8BEAUaDDQxOTkwOTYwODUxMiIMZ%2BzOje5GTy%2F5nz2HKvMEcl8a8GIM47jUmxRN8oUeincmy1rxoMR%2FbxwNNqwoRJ1tg1yIpjCeoMU%2FiCM1AQ9DG7zYjND3M7XMbbDLKXhbHE8YgaV2gmTxh%2BYXYCfub5osH1nDDiLl8ijppZ9hFQ2dLkTT8tXsmQcvk%2BRBN7QaGCwV89Tvk%2BRSlGV8KQdDTB0phnVoV2965Mrf6x6xxy7ZWYsZdGjJuMnVqcadHKiWk0Zo6AxrzzvfyFt%2BXWtNLzdv5EYOt1x1%2BXYYgevtbc2zk5IiU21zM13PAjdHxLHDs6N5U9040fxF11BMmhwqBLgj4pwduTQKjKm5WE5zkIBFqQwk4as5O9ZimUAwehq0MCabSGBjOPbJqZdiUWyUlbbUVePp33%2BL%2FaDmspsjvB7I2lt128jGia%2FTp8l5bbadRa2FYYDXliq%2BxlnnKw2soGQ9JoYRxCLhCttDbJlPu4vcLHJkiRWCxUcKVouJo5h5VI08OZCKEDip7a0gCqO65Ll%2FXJhFwI65fOuwzB24lgK2Wwyrciuuy3V8Ct7PBMgy%2F%2BRr5FHDiYQ9I1Kwqq03ImVIqkoSlYR7oyxoSgxCjH4%2BHWx3E3CwhiejmK8rRW0RRyRd%2FGfXInVOhNq%2Ft%2B9bFvreCibmRPV%2Fmv336NFG6yFH5rao0oeva62BUe0FYvRcQIejshKsrV96FS9u889RXY5hoxKPltCdg9FafIQoGE%2F6xBl0QT5K6RtcvLCVxRt0TZQ4vTSBBj7Pb1VsNLHwVS4zgmdefvDBhk%2FshdPGbJAZuGfCAm5vAEkKiDejc32eDJovgQFwgH9w2Ci%2BG5rS8SdUAuKRO3ZhfjJG0qPDQzw5jocWMKqy%2B74GOpoB6fH8x2x3zK8HwG0Fu5T2CRCVbLvLbhr3O4ByGDfKjzpXCXvfFedKEOKkflbddJoF%2FH4Fiw%2BnrZzU%2BegN%2FopoQ7g%2BWOVsIOYnZLAqM3gseanY9FRvUtgRYIkFfWQyjEXyqDbiz0ZLOQHS6pVXYCfhK3nfWT%2BWWS6my56X3R7P3c4LmbUHdwn%2BsVVZIkGxVYS7vVn7eYDShjvxxw%3D%3D&response-cache-control=max-age%3D21600&X-Amz-Algorithm=AWS4-HMAC-SHA256&X-Amz-Credential=ASIAWDREQERAA4ROHKFF%2F20250322%2Fap-southeast-2%2Fs3%2Faws4_request&X-Amz-Date=20250322T155318Z&X-Amz-SignedHeaders=host&X-Amz-Signature=88032127437a42253e7d2060e69ec51a4465dc210a56d2c4d1123b1996f8709b"",""previewVideoUrl"":null,""accessLevel"":""user"",""status"":""active"",""isActive"":true}]},{""id"":""08dd6957-4d22-49bf-8f57-2f8b5e6e6c40"",""name"":""AutoTestGroup_294640578"",""characters"":[{""id"":""dcf7734d-7b95-4c41-b268-c8db41189286"",""characterType"":""image"",""name"":""Fred.04"",""defaultVoice"":null,""gender"":""genderless"",""previewImageUrl"":""https://genie-ai-ap-southeast-2-419909608512.s3.ap-southeast-2.amazonaws.com/urn%3Abtc0%3Aidentity.bigtincan.org%3ACustomer%3A08da1db3-6ef1-4190-875a-e5713088e84d/avatars/characters/dcf7734d-7b95-4c41-b268-c8db41189286/previewImage.jpg?X-Amz-Expires=21600&X-Amz-Security-Token=IQoJb3JpZ2luX2VjEGgaDmFwLXNvdXRoZWFzdC0yIkcwRQIhAJwEzqn7xb0OabkP57Bq90wRpG5A3IhmNfpjPp%2B453GKAiAXLt8MzJ1uPEy3fFC3hT1ZNXJsKTGq6j5Df%2FAyn1kyciqfBQjB%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F8BEAUaDDQxOTkwOTYwODUxMiIMZ%2BzOje5GTy%2F5nz2HKvMEcl8a8GIM47jUmxRN8oUeincmy1rxoMR%2FbxwNNqwoRJ1tg1yIpjCeoMU%2FiCM1AQ9DG7zYjND3M7XMbbDLKXhbHE8YgaV2gmTxh%2BYXYCfub5osH1nDDiLl8ijppZ9hFQ2dLkTT8tXsmQcvk%2BRBN7QaGCwV89Tvk%2BRSlGV8KQdDTB0phnVoV2965Mrf6x6xxy7ZWYsZdGjJuMnVqcadHKiWk0Zo6AxrzzvfyFt%2BXWtNLzdv5EYOt1x1%2BXYYgevtbc2zk5IiU21zM13PAjdHxLHDs6N5U9040fxF11BMmhwqBLgj4pwduTQKjKm5WE5zkIBFqQwk4as5O9ZimUAwehq0MCabSGBjOPbJqZdiUWyUlbbUVePp33%2BL%2FaDmspsjvB7I2lt128jGia%2FTp8l5bbadRa2FYYDXliq%2BxlnnKw2soGQ9JoYRxCLhCttDbJlPu4vcLHJkiRWCxUcKVouJo5h5VI08OZCKEDip7a0gCqO65Ll%2FXJhFwI65fOuwzB24lgK2Wwyrciuuy3V8Ct7PBMgy%2F%2BRr5FHDiYQ9I1Kwqq03ImVIqkoSlYR7oyxoSgxCjH4%2BHWx3E3CwhiejmK8rRW0RRyRd%2FGfXInVOhNq%2Ft%2B9bFvreCibmRPV%2Fmv336NFG6yFH5rao0oeva62BUe0FYvRcQIejshKsrV96FS9u889RXY5hoxKPltCdg9FafIQoGE%2F6xBl0QT5K6RtcvLCVxRt0TZQ4vTSBBj7Pb1VsNLHwVS4zgmdefvDBhk%2FshdPGbJAZuGfCAm5vAEkKiDejc32eDJovgQFwgH9w2Ci%2BG5rS8SdUAuKRO3ZhfjJG0qPDQzw5jocWMKqy%2B74GOpoB6fH8x2x3zK8HwG0Fu5T2CRCVbLvLbhr3O4ByGDfKjzpXCXvfFedKEOKkflbddJoF%2FH4Fiw%2BnrZzU%2BegN%2FopoQ7g%2BWOVsIOYnZLAqM3gseanY9FRvUtgRYIkFfWQyjEXyqDbiz0ZLOQHS6pVXYCfhK3nfWT%2BWWS6my56X3R7P3c4LmbUHdwn%2BsVVZIkGxVYS7vVn7eYDShjvxxw%3D%3D&response-cache-control=max-age%3D21600&X-Amz-Algorithm=AWS4-HMAC-SHA256&X-Amz-Credential=ASIAWDREQERAA4ROHKFF%2F20250322%2Fap-southeast-2%2Fs3%2Faws4_request&X-Amz-Date=20250322T155318Z&X-Amz-SignedHeaders=host&X-Amz-Signature=986077b9e67fbaae600516dbaae64ba973e9e04a7f9ffc1bde3d4a14cea3e461"",""previewVideoUrl"":null,""accessLevel"":""user"",""status"":""active"",""isActive"":true}]},{""id"":""08dd6957-e49b-4796-85ab-b55c3ff1e694"",""name"":""AutoTestGroup_294894328"",""characters"":[{""id"":""178dbae4-5646-4cd7-812f-d92d9f50e599"",""characterType"":""image"",""name"":""#Fred.04#"",""defaultVoice"":null,""gender"":""genderless"",""previewImageUrl"":""https://genie-ai-ap-southeast-2-419909608512.s3.ap-southeast-2.amazonaws.com/urn%3Abtc0%3Aidentity.bigtincan.org%3ACustomer%3A08da1db3-6ef1-4190-875a-e5713088e84d/avatars/characters/178dbae4-5646-4cd7-812f-d92d9f50e599/previewImage.jpg?X-Amz-Expires=21600&X-Amz-Security-Token=IQoJb3JpZ2luX2VjEGgaDmFwLXNvdXRoZWFzdC0yIkcwRQIhAJwEzqn7xb0OabkP57Bq90wRpG5A3IhmNfpjPp%2B453GKAiAXLt8MzJ1uPEy3fFC3hT1ZNXJsKTGq6j5Df%2FAyn1kyciqfBQjB%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F8BEAUaDDQxOTkwOTYwODUxMiIMZ%2BzOje5GTy%2F5nz2HKvMEcl8a8GIM47jUmxRN8oUeincmy1rxoMR%2FbxwNNqwoRJ1tg1yIpjCeoMU%2FiCM1AQ9DG7zYjND3M7XMbbDLKXhbHE8YgaV2gmTxh%2BYXYCfub5osH1nDDiLl8ijppZ9hFQ2dLkTT8tXsmQcvk%2BRBN7QaGCwV89Tvk%2BRSlGV8KQdDTB0phnVoV2965Mrf6x6xxy7ZWYsZdGjJuMnVqcadHKiWk0Zo6AxrzzvfyFt%2BXWtNLzdv5EYOt1x1%2BXYYgevtbc2zk5IiU21zM13PAjdHxLHDs6N5U9040fxF11BMmhwqBLgj4pwduTQKjKm5WE5zkIBFqQwk4as5O9ZimUAwehq0MCabSGBjOPbJqZdiUWyUlbbUVePp33%2BL%2FaDmspsjvB7I2lt128jGia%2FTp8l5bbadRa2FYYDXliq%2BxlnnKw2soGQ9JoYRxCLhCttDbJlPu4vcLHJkiRWCxUcKVouJo5h5VI08OZCKEDip7a0gCqO65Ll%2FXJhFwI65fOuwzB24lgK2Wwyrciuuy3V8Ct7PBMgy%2F%2BRr5FHDiYQ9I1Kwqq03ImVIqkoSlYR7oyxoSgxCjH4%2BHWx3E3CwhiejmK8rRW0RRyRd%2FGfXInVOhNq%2Ft%2B9bFvreCibmRPV%2Fmv336NFG6yFH5rao0oeva62BUe0FYvRcQIejshKsrV96FS9u889RXY5hoxKPltCdg9FafIQoGE%2F6xBl0QT5K6RtcvLCVxRt0TZQ4vTSBBj7Pb1VsNLHwVS4zgmdefvDBhk%2FshdPGbJAZuGfCAm5vAEkKiDejc32eDJovgQFwgH9w2Ci%2BG5rS8SdUAuKRO3ZhfjJG0qPDQzw5jocWMKqy%2B74GOpoB6fH8x2x3zK8HwG0Fu5T2CRCVbLvLbhr3O4ByGDfKjzpXCXvfFedKEOKkflbddJoF%2FH4Fiw%2BnrZzU%2BegN%2FopoQ7g%2BWOVsIOYnZLAqM3gseanY9FRvUtgRYIkFfWQyjEXyqDbiz0ZLOQHS6pVXYCfhK3nfWT%2BWWS6my56X3R7P3c4LmbUHdwn%2BsVVZIkGxVYS7vVn7eYDShjvxxw%3D%3D&response-cache-control=max-age%3D21600&X-Amz-Algorithm=AWS4-HMAC-SHA256&X-Amz-Credential=ASIAWDREQERAA4ROHKFF%2F20250322%2Fap-southeast-2%2Fs3%2Faws4_request&X-Amz-Date=20250322T155318Z&X-Amz-SignedHeaders=host&X-Amz-Signature=9a4ca15290f551f78d059e584cd107f11473b8662908aa4ac914f198d8e881e4"",""previewVideoUrl"":null,""accessLevel"":""user"",""status"":""active"",""isActive"":true}]}]";
            var expectedJson1 = "";
            var result = JsonExtractor.Extract(Json1);
            Assert.AreEqual(21392, result.Length);
        }

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