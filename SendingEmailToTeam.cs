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
                //from["partyid"] = new EntityReference("systemuser", Context.UserId);
                ////from the customer to ItAdmin
                from["partyid"] = new EntityReference("systemuser", Context.UserId);
                //var retriveusersFromTeam= RetrievingMembersFromTeam(service);
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
                //List<Guid> userGuid = new List<Guid>();
                //string Subject = contact.GetAttributeValue<string>("new_name");
                //string Description = contact.GetAttributeValue<string>("new_description");
                foreach (var usersresult in result.Entities)
                {
                    Guid userid = usersresult.GetAttributeValue<Guid>("systemuserid");
                    //userGuid.Add(id);
                    Entity to = new Entity("activityparty");
                    to["partyid"] = new EntityReference("systemuser", userid);

                    //created a webresource named as "new_RealMadrid" which stores the image file
                    //string webresource = "<html> <img  src='~/WebResources/new_RealMadrid'  width='205' height='150'></html>";
                    // Create Email
                    Entity Email = new Entity("email");
                    Email["from"] = new Entity[] { from };
                    Email["to"] = new Entity[] { to };
                    Email["subject"] = "A new service request is created";
                   // Email["description"] = Description;

                    // Send email request
                    Guid _emailId = service.Create(Email);
                    SendEmailRequest reqSendEmail = new SendEmailRequest();
                    reqSendEmail.EmailId = _emailId;
                    reqSendEmail.TrackingToken = "";
                    reqSendEmail.IssueSend = true;

                    SendEmailResponse res = (SendEmailResponse)service.Execute(reqSendEmail);
                }

                //Entity to = new Entity("activityparty");
                //to["partyid"] = new EntityReference("systemuser", retriveusersFromTeam.id);

                ////created a webresource named as "new_RealMadrid" which stores the image file
                //string webresource = "<html> <img  src='~/WebResources/new_RealMadrid'  width='205' height='150'></html>";
                //// Create Email
                //Entity Email = new Entity("email");
                //Email["from"] = new Entity[] { from };
                //Email["to"] = new Entity[] { to };
                //Email["subject"] = "Welcome Mr./ Mrs " + Name;
                //Email["description"] = "<h3> Dear  " + Name + "</h3>" + "<br/>" + "Welcome to my blog. Hope it helped you." + "<br/>" + " " + "<br/>" + "visit my blog for any query" + "" + "<br/>" + webresource;

                //// Send email request
                //Guid _emailId = service.Create(Email);
                //SendEmailRequest reqSendEmail = new SendEmailRequest();
                //reqSendEmail.EmailId = _emailId;
                //reqSendEmail.TrackingToken = "";
                //reqSendEmail.IssueSend = true;

                //SendEmailResponse res = (SendEmailResponse)service.Execute(reqSendEmail);


            }
            ////////////////////////////////////////
            // to newly created lead user
            //Entity to = new Entity("activityparty");
            //    to["partyid"] = new EntityReference("systemuser", retriveusersFromTeam.id);

            //    //created a webresource named as "new_RealMadrid" which stores the image file
            //    string webresource = "<html> <img  src='~/WebResources/new_RealMadrid'  width='205' height='150'></html>";
            //    // Create Email
            //    Entity Email = new Entity("email");
            //    Email["from"] = new Entity[] { from };
            //    Email["to"] = new Entity[] { to };
            //    Email["subject"] = "Welcome Mr./ Mrs " + Name;
            //    Email["description"] = "<h3> Dear  " + Name + "</h3>" + "<br/>" + "Welcome to my blog. Hope it helped you." + "<br/>" + " " + "<br/>" + "visit my blog for any query" + "" + "<br/>" + webresource;

            //    // Send email request
            //    Guid _emailId = service.Create(Email);
            //    SendEmailRequest reqSendEmail = new SendEmailRequest();
            //    reqSendEmail.EmailId = _emailId;
            //    reqSendEmail.TrackingToken = "";
            //    reqSendEmail.IssueSend = true;

            //    SendEmailResponse res = (SendEmailResponse)service.Execute(reqSendEmail);
            }
        

    //    public ArrayList RetrievingMembersFromTeam(IOrganizationService service)
    //    {
    //       //List<string> users = new List<string>();
           
    //            var users = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>
    //            <entity name='systemuser'>
    //                    <attribute name='fullname' />
    //                    <attribute name='businessunitid' />
    //                    <attribute name='title' />
    //                    <attribute name='address1_telephone1' />
    //                    <attribute name='positionid' />
    //                    <attribute name='systemuserid' />
    //                         <order attribute='fullname' descending='false' />
    //                                  <link-entity name='teammembership' from='systemuserid' to='systemuserid' visible='false' intersect='true'>
    //                                    <link-entity name='team' from='teamid' to='teamid' alias='ac'>
    //                                      <filter type='and'>
    //                                        <condition attribute='teamid' operator='eq' uiname='IT Infrastructure Support Team' uitype='team' value='{07F9344A-8002-E811-A885-002248005556}' />
    //                                      </filter>
    //                                  </link-entity>
    //                                    </link-entity>
    //            </entity>
    //                </fetch>";
    //            var result = service.RetrieveMultiple(new FetchExpression(users));
    //        List<Guid> userGuid = new List<Guid>();
    //        foreach(var usersresult in result.Entities)
    //        {
    //            Guid id = usersresult.GetAttributeValue<Guid>("systemuserid");
    //            userGuid.Add(id);
    //        }
               

    //}
            
        }
    }

