using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiringFunnel
{

    public enum SeniorityLevel : byte
    {
        Entry = 0,
        Junior = 1,
        Medior = 2,
        Senior = 5
    }

    public enum UserLevel : byte
    {
        Interviewer,
        HR,
        Admin
    }

    public enum AnnotationType : byte
    {
        Contact_Static,
        Contact_Comment,
        Interview_Feedback,
        Test_Feedback,
        TestDefense_Feedback,
        Offer_Comment,
        Acceptance_State,
        Rejection_State,
        Quit_State
    }

    public enum Phase : byte
    {
        Contact_Comment,
        Contact_Phase,
        Interview_Phase,
        Test_Phase,
        TestDefense_Phase,
        Offer_Phase,
        Acceptance_Phase,
        Rejection_Phase,
        Quit_Phase
    }

}
