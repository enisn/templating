using System;
using Microsoft.TemplateEngine.Abstractions;
using Microsoft.TemplateEngine.Core.Contracts;
using Microsoft.TemplateEngine.Orchestrator.RunnableProjects.Macros.Config;
using Newtonsoft.Json.Linq;

namespace Microsoft.TemplateEngine.Orchestrator.RunnableProjects.Macros
{
    internal class NowMacro : IMacro
    {
        public Guid Id => new Guid("F2B423D7-3C23-4489-816A-41D8D2A98596");

        public string Type => "now";
         
        public void EvaluateConfig(IVariableCollection vars, IMacroConfig rawConfig, IParameterSet parameters, ParameterSetter setter)
        {
            NowMacroConfig config = rawConfig as NowMacroConfig;

            if (config == null)
            {
                throw new InvalidCastException("Couldn't cast the rawConfig as NowMacroConfig");
            }

            DateTime time = config.Utc ? DateTime.UtcNow : DateTime.Now;
            string value = time.ToString(config.Action);
            Parameter p = new Parameter
            {
                IsVariable = true,
                Name = config.VariableName
            };

            setter(p, value);
        }

        public void EvaluateDeferredConfig(IVariableCollection vars, IMacroConfig rawConfig, IParameterSet parameters, ParameterSetter setter)
        {
            GeneratedSymbolDeferredMacroConfig deferredConfig = rawConfig as GeneratedSymbolDeferredMacroConfig;

            if (deferredConfig == null)
            {
                throw new InvalidCastException("Couldn't cast the rawConfig as a GeneratedSymbolDeferredMacroConfig");
            }

            string action;
            if (!deferredConfig.Parameters.TryGetValue("action", out action))
            {
                throw new ArgumentNullException("action");
            }

            bool utc;
            string utcString;
            if (deferredConfig.Parameters.TryGetValue("utc", out utcString))
            {
                utc = Convert.ToBoolean(utcString);
            }
            else
            {
                utc = false;
            }

            IMacroConfig realConfig = new NowMacroConfig(deferredConfig.VariableName, action, utc);
            EvaluateConfig(vars, realConfig, parameters, setter);
        }

        public void Evaluate(string variableName, IVariableCollection vars, JObject def, IParameterSet parameters, ParameterSetter setter)
        {
            string format = def.ToString("action");
            bool utc = def.ToBool("utc");
            DateTime time = utc ? DateTime.UtcNow : DateTime.Now;
            string value = time.ToString(format);
            Parameter p = new Parameter
            {
                IsVariable = true,
                Name = variableName
            };

            setter(p, value);
        }
    }
}