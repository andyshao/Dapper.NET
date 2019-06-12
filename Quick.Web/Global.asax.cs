using Autofac;
using Autofac.Integration.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Quick.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            AutoFacRegister();
        }

        /// <summary>
        /// 使用AutoFac实现依赖注入
        /// </summary>
        private void AutoFacRegister()
        {
            var builder = new ContainerBuilder();
            SetupResolveRules(builder);  //注入

            builder.RegisterControllers(Assembly.GetExecutingAssembly())   // 注入所有Controller
                .PropertiesAutowired(PropertyWiringOptions.PreserveSetValues); // 开启属性注入方式  
            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }

        private void SetupResolveRules(ContainerBuilder builder)
        {
            //只需要引用接口层IServices，和IRepository 。
            // 具体实现层可以通过拷贝dll或者通过修改输出dll的路径为相对路径，比如：..\Quick.Web\bin\（推荐该方式）。
            // 当然也可以选择将实现层一并引用进来，不过这样比较消耗性能（偷懒方式）。

            //根据名称约定（服务层的接口和实现均以Service结尾），实现服务接口和服务实现的依赖
            builder.RegisterAssemblyTypes(Assembly.Load("Quick.Services"))
              .Where(t => t.Name.EndsWith("Services"))
              .AsImplementedInterfaces(); //把该具体实现类注册为它的所有接口

            //根据名称约定（数据访问层的接口和实现均以Repository结尾），实现数据访问接口和数据访问实现的依赖
            builder.RegisterAssemblyTypes(Assembly.Load("Quick.Repository"))
              .Where(t => t.Name.EndsWith("Repository"))
              .AsImplementedInterfaces()  //把该具体实现类注册为它的所有接口
              .PropertiesAutowired(PropertyWiringOptions.PreserveSetValues); // 开启属性注入方式 
        }
    }
}
