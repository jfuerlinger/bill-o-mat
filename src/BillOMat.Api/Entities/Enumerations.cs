namespace BillOMat.Api.Entities
{
    public static class Enumerations
    {
        public static class Invoice
        {
            public enum Status
            {
                Neu = 0,

                EingereichtOeGk = 10,
                EingereichtMerker = 11,

                TeilweiseVerguetet = 20,
                VollstaendigVerguetet = 21,

                Abgelehnt = 30
            }
        }
    }
}
