using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Security.AccessControl;

namespace LoadAssembly
{
    class Program
    {
        static void Main(string[] args)
        {
            var currentFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var file = Path.Combine(currentFolder, "Library1.dll");
            
            
            if (!File.Exists(file))
            {
                Console.WriteLine("The File doesn't exist. Exit.");
                return;
            }

            var name = AssemblyName.GetAssemblyName(file);
            var ass = Assembly.Load(name);

            var type = ass.GetType("Library1.DataSvc");
            if (type == null)
            {
                Console.WriteLine("Could not find the library");
                return;
            }

            var dictionary = new Dictionary<string, MethodInfo>();
            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance).Where(mi => mi.Name == "Subscribe");


            var personChanged = ass.GetType(DataSvcProxy.PersonChangedTypeName);

            if (personChanged == null)
            {
                return;
            }
            
           
            
            var proxy = new DataSvcProxy(type);
            
            proxy.SubscribeToOne();
            proxy.InvokeOne();
            
            proxy.SubscribeToArr();
            proxy.InvokeArray();
            
            proxy = new DataSvcProxy(type);
            
            proxy.SubscribeToOne();
            proxy.InvokeOne();
            
            proxy.SubscribeToArr();
            proxy.InvokeArray();
            
            Console.WriteLine("Ok.");

        }
    }
}