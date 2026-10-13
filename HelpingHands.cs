namespace Township_API
{
    public static class HelpingHands
    {

        public static string  GetLastModifieds()
        {

            //string ModifiedModules; string ModifiedDetails; int modufiedBy =0;
            String S = "";
            S = S + "";
            S = S + " Date-Version :         |   ModifiedModules: |              ModifiedDetails:                        |   modufiedBy /n";
            S = S + " 13102026 API  v 1.0.1  |  All Modules       |  Deleted/isActive condition is removed handled onUI  |   Nana       /n";
            S = S + " 13102026 API  v 1.0.1  |  CardHistoryReport |  CardHistoryReport   API added                       |   Nana       /n";


            S = S + " 10022026 API  v 1.0.1  |  Contractor        |  Contractor Module   API added                       |   Nana       /n";
            S = S + " 10022026 API  v 1.0.1  |  Profile set       |  Profile Module   API added                          |   Nana       /n";
            S = S + " 10022026 API  v 1.0.1  |  Contractor        |  Singel Default access Added (NRDAccess)|   Nana     |   Nana       /n";

            S = S + " 20022026 API  v 1.1.0  |  Contractor        |  Resident Delete and add validate "+
                                                                    "only one flatno owner is active changed in profile User wise profile change   Nana       /n";

            return S;
        }

    }
}
