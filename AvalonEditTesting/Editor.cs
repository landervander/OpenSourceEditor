using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml.Linq;
using AvalonEditTesting.CompletionWindow;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using Newtonsoft.Json.Linq;
using NLua;

namespace AvalonEditTesting
{
    public class Editor : TextEditor
    {
        private BetterCompletionWindow completionWindow;
        private static Lua lua = new Lua();
        private ObservableCollection<CompletionData> completionData = new ObservableCollection<CompletionData>();
        private bool showing = false;
        private string whole_line = "";
        private string active_typing = "";
        private string last_chars = "";
        public Editor()
        {
            base.ShowLineNumbers = true;
            base.TextArea.TextEntered += TextArea_TextEntered;
            base.TextArea.PreviewKeyDown += TextArea_KeyDown;
            completionWindow = new BetterCompletionWindow(base.TextArea);
            completionWindow.HideCompletionWindow();

            Thread thread = new Thread(new ThreadStart(ReadL));
            thread.Start();
        }

        private void TextArea_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back)
            {
                if(Keyboard.IsKeyDown(Key.LeftCtrl))
                {
                    completionWindow.HideCompletionWindow();
                    active_typing = "";
                }
                if(base.TextArea.Selection.GetText() != "")
                {
                    if (base.TextArea.Selection.Length > active_typing.Length)
                    {
                        active_typing = "";
                        return;
                    }
                    active_typing = active_typing.Remove(active_typing.Length - base.TextArea.Selection.Length, base.TextArea.Selection.Length);
                    Console.WriteLine("typing now: " + active_typing);
                }
                else
                {
                    try
                    {
                        active_typing = active_typing.Remove(active_typing.Length - 1, 1);
                    } catch (Exception ex)
                    {
                        Console.WriteLine("line was empty");
                    }
                    Console.WriteLine("typing now: " + active_typing);
                }
            }
            if(e.Key == Key.Space)
            {
                whole_line += active_typing + " ";
                last_chars = active_typing;
                active_typing = "";
            }
            if(e.Key == Key.Enter)
            {
                whole_line = "";
                last_chars = "";
                active_typing = "";
            }
        }

        private void TextArea_TextEntered(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            if (e.Text == " ") return;
            active_typing += e.Text.Trim();
            Console.WriteLine("whole line: " + whole_line);
            Console.WriteLine("last thing typed: " + last_chars);
            Console.WriteLine("typing now: " + active_typing);
            Console.WriteLine("selection: ");

            completionWindow.ShowCompletionWindow();
            completionWindow.Filter(active_typing);
        }

        private void ReadL()
        {
            var json = File.ReadAllText("D:\\Users\\AleXandar\\Desktop3\\acestuff\\AvalonTesting\\AvalonEditTesting\\AvalonEditTesting\\en-us.json");
            JObject obj = JObject.Parse(json);
            int x = 0;
            foreach (KeyValuePair<string, JToken> val in obj)
            {
                var name = val.Key;
                var object_parameters = val.Value;
                JObject parameters = JObject.Parse(object_parameters.ToString());
                string description = "";

                foreach(var param in parameters)
                {
                    if(param.Key == "documentation")
                    {
                        description = param.Value.ToString();
                    }
                }

                var data = new CompletionData();
                
                if (name.Contains("@roblox/global/"))
                {
                    data.Content = name.Replace("@roblox/global/", "");
                    data.Type = ClassType.Global;
                    completionData.Add(new CompletionData { Content = name.Replace("@roblox/global/", ""), Type = ClassType.Global, Description = description });
                }
                if (name.Contains("@roblox/globaltype/"))
                {
                    data.Content = name.Replace("@roblox/globaltype/", "");
                    data.Type = ClassType.GlobalType;
                    completionData.Add(new CompletionData { Content = name.Replace("@roblox/globaltype/", ""), Type = ClassType.GlobalType, Description = description });

                }
                if (name.Contains("@roblox/enums/"))
                {
                    data.Content = name.Replace("@roblox/enums/", "");
                    data.Type = ClassType.Enum;
                    completionData.Add(new CompletionData { Content = name.Replace("@roblox/enums/", ""), Type = ClassType.Enum, Description = description });

                }
            }           
            completionWindow.InitializeCompletionDatabase(completionData);
        }
    }
}
