namespace Shared.Classes
{
    internal static class XmlData
    {
        public static string Stores =
             @"<Root>
                      <Store>
                        <Products>
                          <Product>
                            <Name>Soap</Name>
                            <Cost>20.00</Cost>
                          </Product>
                          <Product>
                            <Name>Tennis Balls</Name>
                            <Cost>5.00</Cost>
                          </Product>
                          <Product>
                            <Name>Towels</Name>
                            <Cost>15.00</Cost>
                          </Product>
                        </Products>
                        <Customers>
                          <Customer>
                            <Name>Bob</Name>
                            <City>Springfield</City>
                            <Age>25</Age>
                            <State>OH</State>
                            <Country>USA</Country>
                          </Customer>
                          <Customer>
                            <Name>Steve</Name>
                            <City>Albequerqe</City>
                            <Age>20</Age>
                            <State>NM</State>
                            <Country>USA</Country>
                          </Customer>
                          <Customer>
                            <Name>Mary</Name>
                            <City>Albequerqe</City>
                            <Age>20</Age>
                            <State>NM</State>
                            <Country>USA</Country>
                          </Customer>
                          <Customer>
                            <Name>Joy</Name>
                            <City>Albequerqe</City>
                            <Age>60</Age>
                            <State>NM</State>
                            <Country>USA</Country>
                          </Customer>
                          <Customer>
                            <Name>Martha</Name>
                            <City>Little Rock</City>
                            <Age>32</Age>
                            <State>AR</State>
                            <Country>USA</Country>
                          </Customer>
                          <Customer>
                            <Name>Bianca</Name>
                            <City>Dallas</City>
                            <Age>27</Age>
                            <State>TX</State>
                            <Country>USA</Country>
                          </Customer>
                          <Customer>
                            <Name>Tara</Name>
                            <City>London</City>
                            <Age>50</Age>
                            <Country>UK</Country>
                          </Customer>
                        </Customers>
                      </Store>
                    </Root>";

        public static string Keyboards = @"
                <Keyboard>
                    <Name>K70</Name>
                    <SwitchType>Cherry MX Red</SwitchType>
                    <Company>Corsair</Company>
                    <Price>229.99</Price>
                </Keyboard>".Trim();
    }
}