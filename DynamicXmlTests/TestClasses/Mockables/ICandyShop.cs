namespace DynamicXmlTests.TestClasses
{
    public interface ICandyShop
    {
        ICandy GetTopSellingCandy();
        void BuyCandy(ICandy lollipop);
        ICandy GetLollipop();
    }
}