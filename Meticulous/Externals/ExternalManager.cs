using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;

namespace Meticulous.Externals
{
    /// <summary>
    /// 
    /// </summary>
    public class ExternalManager
    {
        private static readonly ExternalManager s_manager = new ExternalManager();

        private ImmutableArray<ExternalDriver> _drivers;
        private readonly ExceptionHandler _exceptionHandler = ExceptionHandler.AlwaysHandling;

        //private readonly Dictionary<IExternal> 


        private ExternalManager()
        {
            _drivers = ImmutableArray<ExternalDriver>.Empty;
        }

        public static ExternalManager Instance
        {
            get { return s_manager; }
        }

        public void Setup(IEnumerable<ExternalDriver> drivers)
        {
            Check.ArgumentNotNull(drivers, "drivers");

            _drivers = drivers.ToImmutableArray();
            var domain = AppDomain.CurrentDomain;
            domain.AssemblyLoad += HandleCurrentDomainAssemblyLoad;

            var asms = domain.GetAssemblies();
            foreach (var asm in asms)
            {
                HandleAssembly(asm);
            }
        }

        public ImmutableArray<ExternalDriver> Drivers
        {
            get { return _drivers; }
        }

        internal void AttachDynamicValue<T>(External<T> value)
        {
            var driver = _drivers.FirstOrDefault(d => d.Supports(typeof (T), value.Path));
            if (driver != null)
            {
                driver.AddDynamicValue(value);
            }
        }

        private void HandleAssembly(Assembly assembly)
        {
            try
            {
                HandleAssembly_Unsafe(assembly);
            }
            catch (Exception e)
            {
                _exceptionHandler.HandleException(e);
            }
        }

        private void HandleAssembly_Unsafe(Assembly assembly)
        {
            var asmAttr = assembly.GetCustomAttribute<ExternalGroupAttribute>();
            if (asmAttr == null)
                return;

            if (!_drivers.Any(d => d.Supports(asmAttr)))
                return;

            var groupStack = ImmutableList.Create(asmAttr);

            var types = assembly.GetTypes();
            foreach (var type in types)
            {
                HandleType(type, groupStack);
            }
        }

        private void HandleType(Type type, ImmutableList<ExternalGroupAttribute> groupStack)
        {
            if (!type.IsClass)
                return;

            var stack = groupStack;
            var groupAttr = type.GetCustomAttribute<ExternalGroupAttribute>(false);
            if (groupAttr != null)
                stack = stack.Add(groupAttr);

            var members = type.GetMembers(BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).Where(m => m is FieldInfo || m is PropertyInfo).ToList();
            foreach (var member in members)
            {
                HandleMember(member, stack);
            }
        }

        private void HandleMember(MemberInfo member, ImmutableList<ExternalGroupAttribute> groupStack)
        {
            try
            {
                HandleMember_Unsafe(member, groupStack);
            }
            catch (Exception e)
            {
                _exceptionHandler.HandleException(e);
            }
        }

        private void HandleMember_Unsafe(MemberInfo member, ImmutableList<ExternalGroupAttribute> groupStack)
        {
            IExternal external = null;
            var attr = member.GetCustomAttribute<ExternalAttribute>();
            var mw = MemberWrapper.Create(member, null);
            var memeberType = mw.Type;
            if (memeberType.IsGenericType)
            {
                var definition = memeberType.GetGenericTypeDefinition();
                if (!definition.IsBaseOrTypeOf(typeof(External<>)))
                    return;

                if (!mw.IsStatic)
                    return;

                external = (IExternal)mw.GetValue();
                if (external == null)
                {
                    external = (IExternal)Activator.CreateInstance(memeberType);
                    mw.SetValue((object)external);
                }
            }
            else if (attr != null)
            {
                external = new ExternalMemberWrapper(mw);
            }

            if (external != null)
                HandleMember(external, attr);
        }

        private void HandleMember(IExternal external, ExternalAttribute attr)
        {
            var type = external.UnderlyingType;
            var uri = new Uri(attr.Path);
            var driver = _drivers.FirstOrDefault(d => d.Supports(type, uri));
            if (driver != null)
            {
                var settings = driver.CreateSettings(attr.RawSettings);
                external.Setup(uri, settings);
                driver.AddStaticValue(external);
            }
        }


        private void HandleCurrentDomainAssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            HandleAssembly(args.LoadedAssembly);
        }
    }
}
