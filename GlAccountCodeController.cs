using GM.DataAccess.UnitOfWork;
using GM.Model.Common;
using GM.Model.GLProcess;
using Microsoft.AspNetCore.Mvc;

namespace GM.Service.GLProcess.Controllers
{
    [Route("[controller]")]   
    [ApiController]  
    public class GLAccountCodeController : ControllerBase  
    {
      private readonly IUnitOfWork _uow;   

      public GLAccountCodeController(IUnitOfWork uow)   
      {
            _uow = uow;   
      }  

      [HttpPost]
      [Route("GetGLAccountCodelist")]   
      public ResultWithModel GetGLAccountCodeList(GLAccountCodeModel model)  
      {
            return _uow.GLProcess.GLAccountCode.Get(model);   
      }  

      [HttpPost]  
      [Route("CreateGLAccountCode")]  
      public ResultWithModel CreateGLAccountCode(GLAccountCodeModel model)  
      {
            return _uow.GLProcess.GLAccountCode.Add(model);   
      }

      [HttpPost]
      [Route("UpdateGLAccountCode")]  
      public ResultWithModel UpdateGLAccountCode(GLAccountCodeModel model)  
      {
            return _uow.GLprocess.GLAccountCode.Update(model);   
      }  

      [HttpPost]  
      [Route("DeleteGLAccountCode")]   
      public ResultWithModel DeleteGLAccountCode(GLAccountCode model)  
      {
            return _uow.GLProcess.GLAccountCode.Remove(model);   
      }  

      [HttpGet]  
      [Route("GetDDlAccountPort")]  
      public ResultWithModel GetDDlAccountPort(string port_name)  
      {
            DropdownModel model = new DropdownModel();  
            model.ProcedureName = "GM_DDL_List_Proc";  
            model.DdltTableList = "GM_gl_rp_port_account_code";   
            model.SearchValue = port_name;  
            return _uow.Dropdown.Get(model);  
      }
    }
}