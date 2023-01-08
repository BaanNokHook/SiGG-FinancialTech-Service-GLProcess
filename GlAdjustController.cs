using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using GM.DataAccess.UnitOfWork;
using GM.Model.Common;
using GM.Model.GLProcess;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GM.Service.GLProcess.Controllers
{
   [Route("[controller]")]  
   [ApiController]  
   public class GlAdjustController : ControllerBase  
   {
      private readonly IUnitOfWork _uow;  

      public GlAdjustController(IUnitOfWork uow)   
      {
            _uow = uow;   
      }  

      [HttpPost]  
      [Route("GetGLAdjustList")]  
      public ResultWithModel GetGLAdjustList(GLAdjustModel model)   
      {
            return _uow.GLProcess.GLAdjust.Get(model);   
      }   

      [HttpPost]     
      [Route("CreateGLAdjust")]   
      public ResultWithModel CreateGLAdjust(List<GLAdjustModel> modelList)   
      {
            ResultWithModel res = new ResultWithModel();   
            try 
            {
                  if (modelList != null)   
                  {
                        string adjustNum = GenAdjustNum(modelList[0].posting_date.Value, modelList[0].create_by);   
                        _uow.BeginTransaction();   

                        string transNo = (modelList[0].adjust_type == "TRANS")    
                          ? modelList[0].trans_no
                          : modelList[0].counter_party_id;  

                        foreach (var model in modelList)
                        {
                           model.adjust_num = adjustNum;   
                           model.trans_no = transNo;   
                           res = _uow.GLProcess.GLAdjust.Add(model);  
                           if (!res.Success)   
                           {
                              _uow.Rollback();  
                              throw new Exception(res.Message);  
                           }
                        }
                        _uow.Commit();   
                  }
                  else
                  {
                        throw new Exception("Data not found.");   
                  }  
            }  
            catch (Exception ex)   
            {
                  return new ResultWithModel
                  {
                        Success = false,  
                        Message = ex.Message   
                  };   
            }  

            return res;   
      }

      [HttpPost]  
      [Route("UpdateGLAdjust")]    
      public ResultWithModel UpdateGLAdjust(List<GLAdjustModel> modelList)        
      {
            ResultWithModel res = new ResultWithModel();  
            try
            {
                  if (modelList != null)  
                  {
                      _uow.BeginTransaction();
                      // Delete old data  
                      res = _uow.GLProcess.GLAdjust.Remove(modelList.FirstOrDefault());  
                      if (!res.Success)   
                      {
                          _uow.Rollback();  
                          throw new Exception(res.Message);   
                      }  
                  }   
                  _uow.Commit();   
            }
            else 
            {
                  throw new Exception("Data not found.");    
            }   
      }
      catch (Exception ex)    
      {
            return new ResultWithModel
            {
                  Success = false,  
                  Message = ex.Message   
            };  
      }  

            return res;   
      }

        [HttpPost]  
        [Route("DeleteGLAdjust")]  
        public ResultWithModel DeleteGLAdjust(GLAdjustModel model)
        {
             return _uow.GLProcess.GLAdjust.Remove(model);   
        }  
         
        [HttpPost]
        [Route("GetGLAdjustDetail")]
        public ResultWithModel GetGLAdjustDetail(GLAdjustModel model)
        {
            return _uow.GLProcess.GLAdjust.Find(model);
        }

        [HttpGet]
        [Route("GetDDlAdjustTrans")]
        public ResultWithModel GetDDlAdjustTrans(string trans_date, string trans_no)
        {
            DropdownModel model = new DropdownModel();
            model.ProcedureName = "GM_DDL_List_Proc";
            model.DdltTableList = "GM_GL_ADJUST_TRANS";
            model.SearchValue = trans_date;//yyyyMMdd
            model.SearchValue2 = trans_no;
            return _uow.Dropdown.Get(model);
        }

        [HttpGet]
        [Route("GetDDlAdjustCounterParty")]
        public ResultWithModel GetDDlAdjustCounterParty(string counter_party)
        {
            DropdownModel model = new DropdownModel();
            model.ProcedureName = "GM_DDL_List_Proc";
            model.DdltTableList = "GM_GL_ADJUST_CPTY";
            model.SearchValue = counter_party;
            return _uow.Dropdown.Get(model);
        }

        [HttpGet]
        [Route("GetDDlAdjustCostCenter")]
        public ResultWithModel GetDDlAdjustCostCenter(string search)
        {
            DropdownModel model = new DropdownModel();
            model.ProcedureName = "GM_DDL_List_Proc";
            model.DdltTableList = "GM_costcenter";
            model.SearchValue = search;
            return _uow.Dropdown.Get(model);
        }

        [HttpGet]
        [Route("GetDDlAdjustAccountCode")]
        public ResultWithModel GetDDlAdjustAccountCode(string search)
        {
            DropdownModel model = new DropdownModel();
            model.ProcedureName = "GM_DDL_List_Proc";
            model.DdltTableList = "GM_gl_adjust_acc_code";
            model.SearchValue = search;
            return _uow.Dropdown.Get(model);
        }

        [HttpGet]
        [Route("GetDDlAdjustPort")]
        public ResultWithModel GetDDlAdjustPort(string search)
        {
            DropdownModel model = new DropdownModel();
            model.ProcedureName = "GM_DDL_List_Proc";
            model.DdltTableList = "GM_gl_adjust_port";
            model.SearchValue = search;
            return _uow.Dropdown.Get(model);
        }

        private string GenAdjustNum(DateTime postingDate, string createBy)
        {
            string adjustNum = string.Empty;
            BaseParameterModel parameter = new BaseParameterModel();
            parameter.ProcedureName = "RP_GL_Adjust_540000_GenAdjustNum_Proc";
            parameter.Parameters.Add(new Field { Name = "posting_date", Value = postingDate });
            parameter.Parameters.Add(new Field { Name = "recorded_by", Value = createBy });
            parameter.ResultModelNames.Add("GLAdjustResultModel");
            ResultWithModel rwm = _uow.ExecDataProc(parameter);

            if (rwm.Success)
            {
                DataSet ds = (DataSet)rwm.Data;
                if (ds.Tables[0].Rows.Count == 1)
                {
                    DataTable dt = ds.Tables[0];
                    adjustNum = dt.Rows[0]["adjust_num"].ToString();
                }
            }
            else
            {
                throw new Exception(rwm.Message);
            }

            return adjustNum;
        }
    }
}