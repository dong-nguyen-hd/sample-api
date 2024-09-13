namespace EPAY.AIRWAY.KIOSK.API.Resources.Enums;

public enum PaymentType : byte
{
    Qr = 1,
    EpayWallet = 2,
    Pos = 3,
    //Cash = 4,
    LocalCard = 5,
    GlobalCard = 6,
    BankAccount = 7,
    PayLater = 8,
}