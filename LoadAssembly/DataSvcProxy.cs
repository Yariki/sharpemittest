using System;
using System.Collections;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace LoadAssembly
{
    public class DataSvcProxy
    {
        internal static readonly string PersonChangedTypeName = "Library1.PersonChanged";
        internal static readonly string PersonsChangedTypeName = "Library1.PersonsChanged";
        internal static readonly string PersonsTypeName = "Library1.Person";

        private Type _personChangedType;
        private Type _personsChangedType;
        private Type _personType;
        
        private Type _type;
        
        private object _dataSvc;
        private MethodInfo _subscribeToOne;
        private MethodInfo _subscribeToArray;
        private MethodInfo _invokePerson;
        private MethodInfo _invokePersons;
        private MethodInfo _callback;

        private DynamicMethod _dynamicMethod;
        private DynamicMethod _dynamicMethod2;
        
        public DataSvcProxy(Type type)
        {
            _type = type;
            _dataSvc = Activator.CreateInstance(_type);
            
            _personChangedType = _type.Assembly.GetType(PersonChangedTypeName);
            _personsChangedType = _type.Assembly.GetType(PersonsChangedTypeName);
            _personType = _type.Assembly.GetType(PersonsTypeName);
            

            _subscribeToOne = _type.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .FirstOrDefault(mi => mi.GetParameters().Any(pt => pt.ParameterType == _personChangedType));
            
            _subscribeToArray = _type.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .FirstOrDefault(mi => mi.GetParameters().Any(pt => pt.ParameterType == _personsChangedType));
            
            _invokePerson = _type.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .FirstOrDefault(mi => mi.Name.Equals("InvokePerson",StringComparison.InvariantCultureIgnoreCase));
            
            _invokePersons = _type.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .FirstOrDefault(mi => mi.Name.Equals("InvokePersons",StringComparison.InvariantCultureIgnoreCase));
            
            _callback = typeof(DataSvcProxy).GetMethod("PersonCallback",BindingFlags.Instance | BindingFlags.NonPublic);

            var arrType = Array.CreateInstance(_personType, 1).GetType();
            
            Type[] Args = new[] {typeof(DataSvcProxy), _personType};
            Type[] Args2 = new[] {typeof(DataSvcProxy), arrType};
            
            _dynamicMethod = new DynamicMethod("InternalPersonCallback", null, Args, typeof(DataSvcProxy),true);
            _dynamicMethod2 = new DynamicMethod("InternalPersonCallback2", null, Args2, typeof(DataSvcProxy),true);

            var ilGenerator = _dynamicMethod.GetILGenerator();
            ilGenerator.Emit(OpCodes.Nop);
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Ldarg_1);
            ilGenerator.EmitCall(OpCodes.Call,_callback,null);
            ilGenerator.Emit(OpCodes.Nop);
            ilGenerator.Emit(OpCodes.Ret);
            
            ilGenerator = _dynamicMethod2.GetILGenerator();
            ilGenerator.Emit(OpCodes.Nop);
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Ldarg_1);
            ilGenerator.EmitCall(OpCodes.Call,_callback,null);
            ilGenerator.Emit(OpCodes.Nop);
            ilGenerator.Emit(OpCodes.Ret);
            
            _dynamicMethod.DefineParameter(1, ParameterAttributes.In, "person");
            _dynamicMethod2.DefineParameter(1, ParameterAttributes.In, "persons");

            var str = _dynamicMethod.ToString();
            
        }

        public void SubscribeToOne()
        {
            var cb = _dynamicMethod.CreateDelegate(_personChangedType, this);
            _subscribeToOne.Invoke(_dataSvc,new []{cb});
            
        }

        public void SubscribeToArr()
        {
            var cb = _dynamicMethod2.CreateDelegate(_personsChangedType, this);
            _subscribeToArray.Invoke(_dataSvc,new []{cb});
        }

        public void InvokeOne()
        {
            _invokePerson.Invoke(_dataSvc,null);
        }

        public void InvokeArray()
        {
            _invokePersons.Invoke(_dataSvc, null);
        }

        private void PersonCallback(object result)
        {
            var array = result as IEnumerable;
            if (array != null)
            {
                Console.WriteLine("Array:");
                foreach (var item in array)
                {
                    Console.WriteLine(item);
                }
            }
            else
            {
                Console.WriteLine("One Object:");
                Console.WriteLine(result);    
            }
            
            
        }
        
    }
}