using Il2CppInterop.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TabsBuilderApi.Utils;
using UnityEngine;
using static StarGen;


namespace TabsBuilderApi
{
    namespace Utils
    {
        /// <summary>
        /// enum for TabBuilderType
        /// </summary>
        public enum TabBuilderType
        {
            None = 0,
            After = 1, Before = 2,
        };
        /// <summary>
        /// Sets a Tab object to be registered
        /// </summary>
        [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
        public sealed class TabBuilderAttribute : Attribute
        {
            public string TabName { get; }
            public string[] Before { get; }
            public string[] after { get; }
            public Type ScriptType { get; }

            public string TopSpriteResource { get; }
            public Action<PlayerCustomizationMenu> Callback { get; }

            public TabBuilderAttribute(string tabName, string sprite, Type scriptType, TabBuilderType type, params string[] list)
            {
                TabName = tabName;
                ScriptType = scriptType;
                if (type != 0)
                {
                    if (type == TabBuilderType.Before)
                    {
                        Before = list ?? Array.Empty<string>();
                    }
                    else
                    {
                        after = list ?? Array.Empty<string>();
                    }
                };
                TopSpriteResource = sprite;
            }

            public TabBuilderAttribute(string tabName, string sprite, Action<PlayerCustomizationMenu> callback, TabBuilderType type, params string[] list)
            {
                TabName = tabName;
                if (type != 0)
                {
                    if (type == TabBuilderType.Before)
                    {
                        Before = list ?? Array.Empty<string>();
                    }
                    else
                    {
                        after = list ?? Array.Empty<string>();
                    }
                };
                Callback = callback;
                TopSpriteResource = sprite;
            }

            public void Build(PlayerCustomizationMenu instance, Assembly asm)
            {
                var builder = new TabBuilder(instance)
                    .CreateTab(TabName);

                if (!string.IsNullOrEmpty(TopSpriteResource))
                {
                    Sprite sprite = null;
                    using (Stream stream = asm.GetManifestResourceStream($"{TopSpriteResource}"))
                    {
                        if (stream != null)
                        {
                            byte[] imageData = new byte[stream.Length];
                            stream.Read(imageData, 0, imageData.Length);

                            Texture2D tex = new Texture2D(2, 2);
                            tex.LoadImage(imageData);

                            sprite = Sprite.Create(
                                tex,
                                new Rect(0, 0, tex.width, tex.height),
                                new Vector2(0.5f, 0.5f)
                            );
                        } else
                        {
                            string msg = $"{TopSpriteResource} ):";
                            foreach (var res in asm.GetManifestResourceNames())
                            {
                                msg = ($"{msg}\n{asm.FullName} has {res}"); 
                            }
                            TabsBuilderApi.TabBuilderPlugin.mls.LogFatal($"{TabsBuilderApi.TabBuilderPlugin.Id}: {asm.FullName} : {msg} | failed");
                        }
                    }
                    builder.CreateTop(sprite);
                }
                else
                {
                    builder.CreateTop(null);
                }

                if (ScriptType != null)
                    builder.ReplaceScript(Il2CppType.From(ScriptType));
                else if (Callback != null)
                    builder.SetAction(Callback);

                if (Before != null)
                {
                    foreach (var b in Before)
                    {
                        if (!string.IsNullOrEmpty(b))
                            builder.Before(b);
                        if (builder.insertIndex.HasValue) break;
                    }
                }

                if (after != null)
                {
                    foreach (var b in after)
                    {
                        if (!string.IsNullOrEmpty(b))
                            builder.After(b);
                        if (builder.insertIndex.HasValue) break;
                    }
                }

                builder.Build();
            }
        };
    }
    namespace backend
    {

        public static class TabRegistry
        {
            public static void BuildAll(PlayerCustomizationMenu instance)
            {
                var tabTypes = AppDomain.CurrentDomain.GetAssemblies()
                 .SelectMany(assembly =>
                 {
                     try
                     {
                         return assembly.GetTypes();
                     }
                     catch (ReflectionTypeLoadException e)
                     {
                         return e.Types.Where(t => t != null);
                     }
                     catch
                     {
                         return Enumerable.Empty<Type>();
                     }
                 })
                 .Where(t => t.IsDefined(typeof(TabBuilderAttribute), false))
                 .ToList();
                foreach (var type in tabTypes)
                {
                    var asm = type.Assembly;
                    var attrs = (TabBuilderAttribute[])Attribute.GetCustomAttributes(type, typeof(TabBuilderAttribute));
                    foreach (var attr in attrs)
                    {
                        attr?.Build(instance,asm);
                    }
                }
            }
        }
    }
}
