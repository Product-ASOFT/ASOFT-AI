using ASOFT.Core.API.Controllers;
using ASOFT.Core.API.Versions;
using Google.Cloud.Language.V1;
using Google.Protobuf.Collections;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace ASOFT.API.Core.Controllers
{
    [ApiVersion(SupportApiVersions.V_2_0_Str)]
    [Route("api/v{version:api-version}/Core/Common/[controller]/[action]")]
    [ApiExplorerSettings(GroupName = "Core")]
    public class ScanController : ASOFTBaseController
    {
        public ScanController()
        {

        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<RepeatedField<Entity>> Test()
        {

            var client = LanguageServiceClient.Create();
            var response = await client.AnalyzeEntitiesAsync(new Document()
            {
                Content = @"Prediction results\navery.smith@contoso.com\nhttps://wwwN.contoso.com\nmob: +44 (0)7911 123456\ntel: +44 (0) 20 9876 5432\nfax: +44 (0) 20 6789 2345\nPage # / Field name / Value\nContactNames\nDr. Avery Smith\nDr. Avery Smith\nSenior Researcher\nCloud & AI Department\n1\nJobTitles\nSenior Researcher\n1\nDepartments\nCloud & Al Department\nEmails\navery.smith@contoso.com\n1\nWebsites\nContoso\n2 Kingdom Street\nPaddington, London, W2 6BD\nhttps://www.contoso.com/\n1\nMobilePhones\n+44 (0) 7911 123456\nOtherPhones\n+44 (0) 20 9876 5432\n1\nFaxes\n+44 (0) 20 6789 2345\nAddresses\n2 Kingdom Street Paddington, London, W2 6BD\n1\nCompanyNames\nContoso\n",
                Type = Document.Types.Type.PlainText
            });
            return response.Entities;

        }
    }
}
