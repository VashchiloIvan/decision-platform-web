using Microsoft.AspNetCore.Mvc;

namespace DecisionPlatformWeb.Controllers;

// ImportExportController - контроллер, контролирующий ручки импорта и экспорта условий задачи
// TODO import / export csv
// TODO excel models??
public class ImportExportController: Controller
{
    // [HttpPost("export-xml")]
    // public IActionResult ExportXml([FromBody] TaskCondition taskCondition)
    // {
    //     var stream = new MemoryStream();
    //     XmlDocument document = new XmlDocument();
    //     XmlNode xmlNode = XmlRepositoryUtils.ObjectToXml(document, taskCondition);
    //     document.AppendChild(xmlNode);
    //     document.Save(stream);
    //
    //     stream.Position = 0;
    //     var a = new FileStreamResult(stream, "text/plain")
    //     {
    //         FileDownloadName = "model.xml"
    //     };
    //     return a;
    // }
    //
    // [HttpPost("export-json")]
    // public IActionResult ExportJson([FromBody] TaskCondition taskCondition)
    // {
    //     MemoryStream stream = new MemoryStream();
    //     JsonSerializer.Serialize(stream, taskCondition);
    //
    //     stream.Position = 0;
    //     var a = new FileStreamResult(stream, "text/plain")
    //     {
    //         FileDownloadName = "model.json"
    //     };
    //     return a;
    // }
    //
    // [HttpPost("import")]
    // public IActionResult Import(IFormFile file)
    // {
    //     int nameLength = file.FileName.Length;
    //     if (file.FileName[(nameLength - 4)..].Equals(".xml"))
    //     {
    //         var stream = file.OpenReadStream();
    //         XmlDocument document = new XmlDocument();
    //         document.Load(stream);
    //         TaskCondition taskCondition = XmlRepositoryUtils.XmlNodeToObject<TaskCondition>(document.SelectSingleNode("TaskCondition"),
    //             "TaskCondition");
    //         return Json(taskCondition);
    //     }
    //
    //     if (file.FileName[(nameLength - 5)..].Equals(".json"))
    //     {
    //         TaskCondition? taskCondition = JsonSerializer.Deserialize<TaskCondition>(file.OpenReadStream());
    //         return Json(taskCondition);
    //     }
    //     
    //     return Json(new TaskCondition());
    // }
}