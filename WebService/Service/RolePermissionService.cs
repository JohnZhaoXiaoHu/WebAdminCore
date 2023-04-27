using ApiModel;
using WebUtils.BaseService;
using WebUtils.BaseService;
using SqlSugar;
using WebModel.Entitys;
using WebService.IService;
using WebUtils;

namespace WebService.Service
{
    /// <summary>
    /// RoleMenuButtonServices
    /// </summary>	
    public partial class RolePermissionService : BaseService<RolePermission>, IRolePermissionService
    {
        /// <summary>
        /// ��ȡ��ɫ��Ȩ���б�
        /// </summary>
        /// <returns></returns>
        public async Task<List<PermissionItem>> GetRolePermissions()
        {
            //�˵�Ȩ��
            var menu = Db.Queryable<SysRole>()
                            .LeftJoin<RolePermission>((r, rp) => r.Id == rp.RoleId)
                            .InnerJoin<Menu>((r, rp, m) => rp.PermissionId == m.Id)
                            .InnerJoin<Interface>((r, rp, m, i) => m.Fid == i.Id)
                            .Where((r, rp, m, i) => !r.IsDelete && !rp.IsDelete && !m.IsDelete && !i.IsDelete)
                            .Select((r, rp, m, i) => new PermissionItem()
                            {
                                RoleId = r.Id,
                                Url = i.Url,
                            }).MergeTable();
            // ��ťȨ��
            var button = Db.Queryable<SysRole>()
                            .LeftJoin<RolePermission>((r, rp) => r.Id == rp.RoleId)
                            .InnerJoin<Button>((r, rp, b) => rp.PermissionId == b.Id)
                            .InnerJoin<Interface>((r, rp, b, i) => b.Fid == i.Id)
                            .Where((r, rp, b, i) => !r.IsDelete && !rp.IsDelete && !b.IsDelete && !i.IsDelete)
                            .Select((r, rp, b, i) => new PermissionItem()
                            {
                                RoleId = r.Id,
                                Url = i.Url,
                            }).MergeTable();
            return await Db.UnionAll(menu, button).ToListAsync();
        }

        /// <summary>
        /// ��ȡ������ɫ��Ȩ�������������
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<List<Menu>> GetRoleAuthTree(string id)
        {
            // �˵���
            var menus = await Db.Queryable<Menu>()
                          .LeftJoin<RolePermission>((m, rp) => m.Id == rp.PermissionId && rp.RoleId == id)
                          .Select((m, rp) => new Menu()
                          {
                              Id = m.Id,
                              Pid = m.Pid,
                              Name = m.Name,
                              Description = m.Description,
                              Sort = m.Sort,
                              IsDelete = m.IsDelete,
                              Buttons = SqlFunc.Subqueryable<Button>()
                                                .LeftJoin<RolePermission>((b, rp) => b.Id == rp.PermissionId)
                                                .Where((b, rp) => b.Mid == m.Id)
                                                .ToList((b, rp) => new Button()
                                                {
                                                    Id = b.Id,
                                                    Mid = b.Mid,
                                                    Name = b.Name,
                                                    Description = b.Description,
                                                    Sort = b.Sort,
                                                    Selected = rp.RoleId == id ? true : false,
                                                    IsDelete = b.IsDelete,
                                                })
                          }).MergeTable().OrderBy(t => t.Sort)
                          .ToTreeAsync(t => t.Children, t => t.Pid, "");
            return menus;
        }
        /// <summary>
        /// ��ȡ��ɫ����Ȩ������Ҷ�ӽڵ�
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public async Task<List<string>> GetRoleAuthLeafChecked(string roleId)
        {
            var list = new List<string>();
            if (roleId.IsNotEmpty())
            {
                //���Ҳ˵���Ҷ�ӽڵ㣬el-tree����Ҫ�����ϼ��ڵ�, ֻ��ѯ����Ȩ�Ĳ˵�Id
                var query1 = await Db.Queryable<Menu>().LeftJoin<Menu>((a, b) => a.Id == b.Pid)
                                .LeftJoin<RolePermission>((a, b, c) => a.Id == c.PermissionId)
                                .Where((a, b, c) => b.Id == null && c.RoleId == roleId)
                                //.Select((a, b, c) => (object)a.Id);
                                .Select((a, b, c) => a.Id).ToListAsync();
                //ֻ������Ȩ�İ�ťId
                var query2 = await Db.Queryable<RolePermission>().InnerJoin<Button>((rp, b) => rp.PermissionId == b.Id)
                                .Where((rp, b) => rp.RoleId == roleId && b.Id != null)
                                //.Select(t => (object)t.PermissionId);
                                .Select((rp, b) => rp.PermissionId).ToListAsync();

                //list = await Db.Union(query1, query2).Select<string>().ToListAsync();
                list.AddRange(query1);
                list.AddRange(query2);
            }
            return list;
        }
    }
}
