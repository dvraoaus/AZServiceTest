/*
	'=======================================================================
	'   Author(s):      G. Ferrigno
	'   Module/Form:    CustomClientChannel.cs
	'   Created Date:   
	'   Description:    Helper classes for channell factories 
	'
	'   Modification History:
	'=======================================================================
	'   Author(s)       Date        Control/Procedure       Change
	'=======================================================================
    '   Rao   04/19/2010    Removed Unused class CustomClientChannel<T>
    '   Rao   04/19/2010    Adding Static Class ServicesFactory to cache channel factories
	'=======================================================================
*/

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;

namespace VistaSG.Services
{
     

    public  class CustomChannelFactory<T> : ChannelFactory<T>
    {
        string configurationPath;
        string endpointConfigurationName;

        public CustomChannelFactory(string configurationPath)
            : base(typeof(T))
        {
            this.configurationPath = configurationPath;
            base.InitializeEndpoint((string)null, null);
        }

        public CustomChannelFactory(Binding binding, string configurationPath)
            : this(binding, (EndpointAddress)null, configurationPath)
        {
        }

        public CustomChannelFactory(ServiceEndpoint serviceEndpoint, string configurationPath)
            : base(typeof(T))
        {
            this.configurationPath = configurationPath;
            base.InitializeEndpoint(serviceEndpoint);
        }

        public CustomChannelFactory(string endpointConfigurationName, string configurationPath)
            : this(endpointConfigurationName, null, configurationPath)
        {
        }

        public CustomChannelFactory(Binding binding, EndpointAddress endpointAddress, string configurationPath)
            : base(typeof(T))
        {
            this.configurationPath = configurationPath;
            base.InitializeEndpoint(binding, endpointAddress);
        }

        public CustomChannelFactory(Binding binding, string remoteAddress, string configurationPath)
            : this(binding, new EndpointAddress(remoteAddress), configurationPath)
        {
        }

        public CustomChannelFactory(string endpointConfigurationName, EndpointAddress endpointAddress, string configurationPath)
            : base(typeof(T))
        {
            this.configurationPath = configurationPath;
            this.endpointConfigurationName = endpointConfigurationName;
            base.InitializeEndpoint(endpointConfigurationName, endpointAddress);
        }

        protected override ServiceEndpoint CreateDescription()
        {
            ServiceEndpoint serviceEndpoint = base.CreateDescription();

            if (endpointConfigurationName != null)
                serviceEndpoint.Name = endpointConfigurationName;

            ExeConfigurationFileMap map = new ExeConfigurationFileMap();
            map.ExeConfigFilename = this.configurationPath;

            Configuration config = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);
            ServiceModelSectionGroup group = ServiceModelSectionGroup.GetSectionGroup(config);

            ChannelEndpointElement selectedEndpoint = null;

            foreach (ChannelEndpointElement endpoint in group.Client.Endpoints)
            {
                if (endpoint.Contract == serviceEndpoint.Contract.ConfigurationName &&
                    (this.endpointConfigurationName == null || this.endpointConfigurationName == endpoint.Name))
                {
                    selectedEndpoint = endpoint;
                    break;
                }
            }

            if (selectedEndpoint != null)
            {
                if (serviceEndpoint.Binding == null)
                {
                    string selectedEndPointBindingConfiguration = selectedEndpoint.BindingConfiguration;
                    serviceEndpoint.Binding = CreateBinding(selectedEndpoint.Binding, group, selectedEndPointBindingConfiguration);
                }

                if (serviceEndpoint.Address == null)
                {
                    serviceEndpoint.Address = new EndpointAddress(selectedEndpoint.Address, GetIdentity(selectedEndpoint.Identity), selectedEndpoint.Headers.Headers);
                }

                if (serviceEndpoint.Behaviors.Count == 0 && selectedEndpoint.BehaviorConfiguration != null && selectedEndpoint.BehaviorConfiguration.Length > 0)
                {
                    AddBehaviors(selectedEndpoint.BehaviorConfiguration, serviceEndpoint, group);
                }

                serviceEndpoint.Name = selectedEndpoint.Contract;
            }

            return serviceEndpoint;

        }
        private Binding CreateBinding(string bindingName, ServiceModelSectionGroup group, string bindingConfigurationName)
        {
            BindingCollectionElement bindingElementCollection = group.Bindings[bindingName];
            if (bindingElementCollection.ConfiguredBindings.Count > 0)
            {
                // By default we will use first binding 
                IBindingConfigurationElement be = bindingElementCollection.ConfiguredBindings[0];

                if (bindingConfigurationName.Length > 0)
                {
                    for (int i = 0; i < bindingElementCollection.ConfiguredBindings.Count; i++)
                    {
                        if (bindingElementCollection.ConfiguredBindings[i].Name == bindingConfigurationName)
                        {
                            be = bindingElementCollection.ConfiguredBindings[i];
                            break;
                        }
                    }

                }


                Binding binding = GetBinding(be);
                if (be != null)
                {
                    be.ApplyConfiguration(binding);
                }

                return binding;
            }

            return null;
        }

        private Binding GetBinding(IBindingConfigurationElement configurationElement)
        {
            if (configurationElement is CustomBindingElement)
                return new CustomBinding();
            else if (configurationElement is BasicHttpBindingElement)
                return new BasicHttpBinding();
            else if (configurationElement is NetMsmqBindingElement)
                return new NetMsmqBinding();
            else if (configurationElement is NetNamedPipeBindingElement)
                return new NetNamedPipeBinding();
            else if (configurationElement is NetTcpBindingElement)
                return new NetTcpBinding();
            else if (configurationElement is WSDualHttpBindingElement)
                return new WSDualHttpBinding();
            else if (configurationElement is WSHttpBindingElement)
                return new WSHttpBinding();
            else if (configurationElement is WSFederationHttpBindingElement)
                return new WSFederationHttpBinding();

            return null;
        }

        private void AddBehaviors(string behaviorConfiguration, ServiceEndpoint serviceEndpoint, ServiceModelSectionGroup group)
        {
            EndpointBehaviorElement behaviorElement = group.Behaviors.EndpointBehaviors[behaviorConfiguration];
            for (int i = 0; i < behaviorElement.Count; i++)
            {
                BehaviorExtensionElement behaviorExtension = behaviorElement[i];
                object extension = behaviorExtension.GetType().InvokeMember("CreateBehavior",
                    BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance,
                    null, behaviorExtension, null);
                if (extension != null)
                {
                    serviceEndpoint.Behaviors.Add((IEndpointBehavior)extension);
                }
            }
        }

        private EndpointIdentity GetIdentity(IdentityElement element)
        {
            EndpointIdentity identity = null;
            PropertyInformationCollection properties = element.ElementInformation.Properties;
            if (properties["userPrincipalName"].ValueOrigin != PropertyValueOrigin.Default)
            {
                return EndpointIdentity.CreateUpnIdentity(element.UserPrincipalName.Value);
            }
            if (properties["servicePrincipalName"].ValueOrigin != PropertyValueOrigin.Default)
            {
                return EndpointIdentity.CreateSpnIdentity(element.ServicePrincipalName.Value);
            }
            if (properties["dns"].ValueOrigin != PropertyValueOrigin.Default)
            {
                return EndpointIdentity.CreateDnsIdentity(element.Dns.Value);
            }
            if (properties["rsa"].ValueOrigin != PropertyValueOrigin.Default)
            {
                return EndpointIdentity.CreateRsaIdentity(element.Rsa.Value);
            }
            if (properties["certificate"].ValueOrigin != PropertyValueOrigin.Default)
            {
            }

            return identity;
        }


        protected override void ApplyConfiguration(string configurationName)
        {
            //base.ApplyConfiguration(configurationName);
        }
    }


    public static class ServicesFactory
    {
        private static Dictionary<string, ChannelFactory> _factories;

        static ServicesFactory()
        {
            _factories = new Dictionary<string, ChannelFactory>();
        }

        public static T CreateServiceChannel<T>(string endpointConfigurationName, string serviceConfigurationFileName)
        {
            ChannelFactory<T> factory = null;
            T channel = default(T);
            if ( !string.IsNullOrEmpty(endpointConfigurationName) && !string.IsNullOrEmpty(serviceConfigurationFileName))
            {
                lock (_factories)
                {
                    string key = serviceConfigurationFileName + "!" + endpointConfigurationName;
                    if (!_factories.ContainsKey(key))
                    {
                        factory = new CustomChannelFactory<T>(endpointConfigurationName, serviceConfigurationFileName);
                        if (factory != null)
                        {
                            _factories.Add(key, factory);
                        }
                    }
                    else
                    {
                        factory = _factories[key] as ChannelFactory<T>;
                    }
                }
                if (factory != null)
                {
                    channel = factory.CreateChannel();
                    ((IClientChannel)channel).Open();
                }
            }
            return channel;
        }

        public static void CloseChannel(IClientChannel clientChannel)
        {

            if (clientChannel != null)
            {
                try
                {
                    if (clientChannel.State == CommunicationState.Opened)
                    {
                        clientChannel.Close();
                    }
                    else if (clientChannel.State == CommunicationState.Faulted)
                    {
                        clientChannel.Abort();
                    }

                }
                catch (CommunicationException)
                {
                    clientChannel.Abort();
                }
                catch (TimeoutException)
                {
                    clientChannel.Abort();
                }
                catch (Exception ex)
                {
                    clientChannel.Abort();
                    throw ex;
                }
            }
        }

        public static string GetMachineIP()
        {
            string machineIP = Environment.MachineName; // 
            System.Net.IPHostEntry ipHostEntry = System.Net.Dns.GetHostEntry(Environment.MachineName);
            if (ipHostEntry != null && ipHostEntry.AddressList != null && ipHostEntry.AddressList.Length > 0)
            {
                machineIP = ipHostEntry.AddressList[0].ToString();
            }
            return machineIP;
        }
    }

}
