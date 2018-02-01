using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sending_Email_to_Team
{
    public class SendingEmailToTeam:IPlugin
    {
        public void Execute(IServiceProvider serviceprovider)
        {

            IPluginExecutionContext Context = (IPluginExecutionContext)serviceprovider.GetService(typeof(IPluginExecutionContext));//getting the context from input parameters
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceprovider.GetService(typeof(IOrganizationServiceFactory));//getting service factory 
            IOrganizationService service = (IOrganizationService)serviceFactory.CreateOrganizationService(Context.UserId);//retrieving the service 
            ITracingService tracingService = (ITracingService)serviceprovider.GetService(typeof(ITracingService));// getting the tracing service
            if (Context.InputParameters.Contains("Target") && Context.InputParameters["Target"] is Entity)
            {
                Entity contact = (Entity)Context.InputParameters["Target"];//getting contact entity from inputparameters
                string EmaildId = contact.GetAttributeValue<string>("emailaddress1");//retrieving the Email Id from contact entity
                string Name = contact.GetAttributeValue<string>("lastname");//retrieving last name of contact
                // from current user
                Entity from = new Entity("activityparty");
                from["partyid"] = new EntityReference("systemuser", Context.UserId);
                // Retrieving users from Team here we are referring to "IT Infrastructure Support Team"
                var users = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>
                <entity name='systemuser'>
                        <attribute name='fullname' />
                        <attribute name='businessunitid' />
                        <attribute name='title' />
                        <attribute name='address1_telephone1' />
                        <attribute name='positionid' />
                        <attribute name='systemuserid' />
                             <order attribute='fullname' descending='false' />
                                      <link-entity name='teammembership' from='systemuserid' to='systemuserid' visible='false' intersect='true'>
                                        <link-entity name='team' from='teamid' to='teamid' alias='ac'>
                                          <filter type='and'>
                                            <condition attribute='teamid' operator='eq' uiname='IT Infrastructure Support Team' uitype='team' value='{07F9344A-8002-E811-A885-002248005556}' />
                                          </filter>
                                      </link-entity>
                                        </link-entity>
                </entity>
                    </fetch>";
                var result = service.RetrieveMultiple(new FetchExpression(users));
                foreach (var usersresult in result.Entities)
                {
                    Guid userid = usersresult.GetAttributeValue<Guid>("systemuserid");
                    //userGuid.Add(id);
                    Entity to = new Entity("activityparty");
                    to["partyid"] = new EntityReference("systemuser", userid);
                    // Create Email
                    Entity Email = new Entity("email");
                    Email["from"] = new Entity[] { from };
                    Email["to"] = new Entity[] { to };
                    Email["subject"] = "A new service request is created";
                    // Send email request
                    Guid _emailId = service.Create(Email);
                    SendEmailRequest reqSendEmail = new SendEmailRequest();
                    reqSendEmail.EmailId = _emailId;
                    reqSendEmail.TrackingToken = "";
                    reqSendEmail.IssueSend = true;
                    SendEmailResponse res = (SendEmailResponse)service.Execute(reqSendEmail);
                }
        }
    }

