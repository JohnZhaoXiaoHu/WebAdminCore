using WebExtention.Injection;

var builder = WebApplication.CreateBuilder(args);
// ע��ģ�黯����
builder.AddServiceInjection();
//������ע���Զ������
var service = builder.Services;
var app = builder.Build();
//ʹ�÷���
app.UseAppInjection(builder);
