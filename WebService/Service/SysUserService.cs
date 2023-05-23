using WebUtils.BaseService;
using SqlSugar;
using WebModel.Entitys;
using WebService.IService;

namespace WebService.Service
{
    /// <summary>
    /// SysUserServices
    /// </summary>	
    public partial class SysUserServices : BaseService<SysUser>, ISysUserService
    {
        #region ��ȡ�û���Ϣ
        /// <summary>
        /// ��ѯ�û�Ȩ����Ϣ
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<List<Menu>> GetUserAuth(string id)
        {
            //�ҳ����û������н�ɫID
            var permissionIds = Db.Queryable<SysUser, SysRole, RolePermission>((u, r, rp) => new JoinQueryInfos(
                            JoinType.Inner, SqlFunc.JsonArrayAny(u.RoleIds, r.Id),
                            JoinType.Inner, r.Id == rp.RoleId
                        ))
                        .Where((u, r, rp) => u.Id == id && !u.IsDelete && !r.IsDelete)
                        .Select((u, r, rp) => new { rp.PermissionId }).Distinct().MergeTable();

            //�����û����еĽ�ɫ��Ϣ���ҳ�����Ȩ��Ȩ��Id
            var menus = await Db.Queryable<Menu>().InnerJoin(permissionIds, (m, rp) => m.Id == rp.PermissionId)
                            .Where((m, rp) => !m.IsDelete)
                            .Select((m, rp) => new Menu
                            {
                                Id = m.Id,
                                Pid = m.Pid,
                                Name = m.Name,
                                Path = m.Path,
                                Type = m.Type,
                                Sort = m.Sort,
                                Description = m.Description,
                                Visiable = m.Visiable,
                            }).Distinct().OrderBy(m => m.Sort).ToTreeAsync(m => m.Children, m => m.Pid, "");

            //��ȡ����Ȩ�İ�ť����
            await Db.ThenMapperAsync(menus, async t =>
            {
                t.Buttons = await Db.Queryable<Button>().InnerJoin(permissionIds, (b,rp)=> b.Id == rp.PermissionId)
                                    .Where((b, rp)=>!b.IsDelete).ToListAsync();
            });
            return menus;
        }
        #endregion
    }
}
