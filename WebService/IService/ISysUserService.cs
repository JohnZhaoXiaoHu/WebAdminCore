using WebUtils.BaseService;
using WebUtils;
using System.Linq.Expressions;
using ApiModel;
using WebModel.Entitys;

namespace WebService.IService
{
    /// <summary>
    /// ISysServices
    /// </summary>	
    public partial interface ISysUserService : IBaseService<SysUser>
    {
        #region �û���¼�Լ��û���Ϣ��ȡ
        Task<List<Menu>> GetUserAuth(string id);
        #endregion
    }
}
