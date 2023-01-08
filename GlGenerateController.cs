using GM.DataAccess.UnitOfWork;
using GM.Model.Common;
using GM.Model.GLProcess;
using Microsoft.AspNetCore.Mvc;

namespace GM.Service.GLProcess.Controllers
{
    [Route("[controller]")]   
    [ApiController] 
    public class GLGenerateController : ControllerBase  
    {
      private readonly IUnitOfWork _uow;    

      public GLGenerateController(IUnitOfWork uow)  
      {
            _uow = uow; 
      }

      [HttpPost]   
      [Route("GLGenerateRunBatch")]  
      public ResultWithModel GLGenerateRunBatch(GLGenerateModel model)    
      {
            // Save Code will be here   
            BaseParameterModel parameter = new BaseParameterModel();   
            parameter.ProcedureName = "GM_Batch-Run_GL";    
            parameter.Parameters.Add(new Field { Name = "acc_event", Value = model.event_generate });   
            parameter.Parameters.Add(new Field { Name = "cur", Value = model.cur });   
            parameter.Parameters.Add(new Field { Name = "from_business_date", Value = model.from_date });  
            parameter.Parameters.Add(new Field { Name = "to_business_date", Value = model.to_date });   
            parameter.ResultModelNames.Add("GLGenerateResultModel");     
            return _uow.ExecNonQueryProc(parameter);   
      } 

      [HttpGet]
      [Route("ddlCurrency")]   
      public ResultWithModel DDLCurrency(string text)   
      {
          DropdownModel model = new DropdownModel();  
          model.ProcedureName = "GM-DDL_List_Proc";   
          model.DdltTableList = "GM_Currency";   
          model.SearchValue = text;  
          return _uow.Dropdown.Get(model);   
      }  

      [HttpGet]  
      [Route("ddlevent")]   
      public ResultWithModel DDLEvent(string text)    
      {
          DropdownModel model = new DropdownModel();   
          model.ProcedureName = "GM_DDL_List_Proc";   
          model.DdltTableList = "GM_GL_EVENT_GENERATE";    
          model.SearchValue = text;   
          return _uow.Dropdown.Get(model);     
      }    
    }
}