Add-PSSnapin Microsoft.SharePoint.PowerShell –ErrorAction SilentlyContinue
 
#Custom Function to Create new SharePoint Group
function Create-SPGroup
{  
    param ($GroupName)  

	$SiteURL = "http://sit-icollaborate.moe.gov.sg/spimportersc"
	$PermissionLevel = "Read"
	$GroupDescription = ""
    try
    {
        #Get the Web
        $web = Get-SPWeb -Identity $SiteURL
         
        if($web -ne $null)
        {
            #Check if Group Exists already
            if ($web.SiteGroups[$GroupName] -ne $null)  
            {  
                write-Host "Group $GroupName exists Already!" -ForegroundColor Red 
            }  
            else 
            {  
                #Create SharePoint Group
                $Web.SiteGroups.Add($GroupName, "slnp_spfarm", $web.SiteUsers["schools\slnp_spfarm"], $GroupDescription)  
                #Get the newly created group and assign permission to it
                $Group = $web.SiteGroups[$groupName]  
                $roleAssignment = new-object Microsoft.SharePoint.SPRoleAssignment($group)  
                $roleDefinition = $web.Site.RootWeb.RoleDefinitions[$permissionLevel]  
                $roleAssignment.RoleDefinitionBindings.Add($roleDefinition)  
                $web.RoleAssignments.Add($roleAssignment)  
                $web.Update()  
 
                write-Host "Group: $GroupName created successfully!" -ForegroundColor Green
            }  
  
            $web.Dispose() 
        }
    }
    catch [System.Exception]
    {
        write-host $_.Exception.ToString() -ForegroundColor Red 
    }
}



#Call the function to create 133 Sharepoint group
Create-SPGroup "Admin(Secretary)"
Create-SPGroup "Administrative Executive"
Create-SPGroup "Allied Educator (LBS)"
Create-SPGroup "Allied Educator (TL)"
Create-SPGroup "Allied Educator TL Prog fr Act Learning"
Create-SPGroup "AM (All)"
Create-SPGroup "AM (JC - CI)"
Create-SPGroup "AM (Primary)"
Create-SPGroup "AM (Secondary)"
Create-SPGroup "Assistant Directors"
Create-SPGroup "Associate"
Create-SPGroup "AVA Technician"
Create-SPGroup "Corporate Support Officer"
Create-SPGroup "Deputy DGE"
Create-SPGroup "Deputy Directors"
Create-SPGroup "Deputy Secretary"
Create-SPGroup "Director-General"
Create-SPGroup "Directors"
Create-SPGroup "Directors (D & AD & DD)"
Create-SPGroup "Dy Director (MXS - I)"
Create-SPGroup "Education Workshop Instructor"
Create-SPGroup "Executive"
Create-SPGroup "Executive Assistant"
Create-SPGroup "Head (EDUN SVC - J)"
Create-SPGroup "Head of Dept Larger Portfolio"
Create-SPGroup "HOD (All)"
Create-SPGroup "HOD (JC - CI)"
Create-SPGroup "HOD (Primary)"
Create-SPGroup "HOD (Secondary)"
Create-SPGroup "International Teaching Associate"
Create-SPGroup "Lead School Counsellor (LSC)"
Create-SPGroup "Lead Specialist"
Create-SPGroup "Lead Teacher (LT)"
Create-SPGroup "Level Head"
Create-SPGroup "Librarian"
Create-SPGroup "Management Executive (2008)"
Create-SPGroup "Management Support Officer"
Create-SPGroup "MANAGEMENT SUPPORT OFFICER (Gr II)"
Create-SPGroup "Manager (EAS)"
Create-SPGroup "Master Teacher (MTT)"
Create-SPGroup "Master Teachers"
Create-SPGroup "Operations Manager"
Create-SPGroup "Operations Support Officer"
Create-SPGroup "Operations Support Scheme"
Create-SPGroup "PA (S handwriter - L)"
Create-SPGroup "Partnerships Liaison Manager"
Create-SPGroup "Personal Assistant"
Create-SPGroup "Principal Master Teacher (PMTT)"
Create-SPGroup "Principal Specialist"
Create-SPGroup "Principals"
Create-SPGroup "Principals - All (Ps & VPs & VP(Admin))"
Create-SPGroup "Principals (JC - CI)"
Create-SPGroup "Principals (Primary)"
Create-SPGroup "Principals (Secondary)"
Create-SPGroup "Provisional Snr Specialist"
Create-SPGroup "Quality Assessor"
Create-SPGroup "Research(Exec)"
Create-SPGroup "School Counsellor (SC)"
Create-SPGroup "School Laboratory Technician"
Create-SPGroup "School Staff Developer"
Create-SPGroup "Senior AED (T&L)"
Create-SPGroup "Senior Associate"
Create-SPGroup "Senior Asst Director (MX)(MTI)"
Create-SPGroup "Senior School Counsellor (SSC)"
Create-SPGroup "Senior Specialist Level 1"
Create-SPGroup "Senior Specialist Level 2"
Create-SPGroup "Senior Teachers"
Create-SPGroup "Snr Asst Director (other Ministries)"
Create-SPGroup "Snr Hlth Policy Analyst"
Create-SPGroup "Snr Quality Assessor"
Create-SPGroup "Snr Specialist"
Create-SPGroup "Snr Teacher"
Create-SPGroup "Staff Officer (EAS)"
Create-SPGroup "Staff Officer (Edun Offr)"
Create-SPGroup "Subject Head"
Create-SPGroup "Superintendents"
Create-SPGroup "Supervisor (LC)"
Create-SPGroup "Teacher (EDUN SVC - K)"
Create-SPGroup "Teacher (EDUN SVC - L)"
Create-SPGroup "Teachers (All)"
Create-SPGroup "Teachers (JC - CI)"
Create-SPGroup "Teachers (Primary)"
Create-SPGroup "Teachers (Secondary)"
Create-SPGroup "Technical Manager"
Create-SPGroup "Technical Supervisor"
Create-SPGroup "Technician"
Create-SPGroup "Translator"
Create-SPGroup "TSO"
Create-SPGroup "Vice Principals"
Create-SPGroup "Vice Principals - Admin"
Create-SPGroup "Vice Principals - Admin (JC - CI)"
Create-SPGroup "Vice Principals - Admin (Primary)"
Create-SPGroup "Vice Principals - Admin (Secondary)"
Create-SPGroup "Vice Principals (JC - CI)"
Create-SPGroup "Vice Principals (Primary)"
Create-SPGroup "Vice Principals (Secondary)"
Create-SPGroup "Div 1 Officers"
Create-SPGroup "Div 2 Officers"
Create-SPGroup "Div 3 Officers"
Create-SPGroup "Div 4 Officers"
Create-SPGroup "EO Officers"
Create-SPGroup "EA Officers"
Create-SPGroup "EPD MRD"
Create-SPGroup "School Users"
Create-SPGroup "HQ Users"
Create-SPGroup "Communications Division CEG"
Create-SPGroup "Curriculum Planning & Development Division 1"
Create-SPGroup "Education Services Division"
Create-SPGroup "Educational Technology Division"
Create-SPGroup "Finance & Development Division"
Create-SPGroup "Higher Education Division"
Create-SPGroup "Organisation Development Division"
Create-SPGroup "HR Solutions & Capabilities Division"
Create-SPGroup "Planning Division"
Create-SPGroup "Schools Division"
Create-SPGroup "School Planning & Placement Division"
Create-SPGroup "Academy of Singapore Teachers"
Create-SPGroup "Internal Audit Branch"
Create-SPGroup "Legal Services Branch"
Create-SPGroup "Compulsory Education Unit"
Create-SPGroup "Strategic Comms Branch"
Create-SPGroup "Student Development Curriculum"
Create-SPGroup "Organisational Psychology Branch"
Create-SPGroup "STAR"
Create-SPGroup "ELIS"
Create-SPGroup "PESTA"
Create-SPGroup "Curriculum Planning & Devt Div 2"
Create-SPGroup "Public Affairs Division"
Create-SPGroup "Information Technology Branch"
Create-SPGroup "MOE Recruitment"
Create-SPGroup "Directorate"
Create-SPGroup "Organisation Development Div"