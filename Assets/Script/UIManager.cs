using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI pescadoA, pescadoB, pescadoC, espada, escudo, hacha, tarta, torta, pan, galletas, banana, zanahoria, sandia, carne, jamon, chorizo;
    public Stand pescado, vegetales, procesado, armaduras, carnes;

    void Update()
    {
        var counterPescado = Counter(pescado);
        var counterVegetales = Counter(vegetales);
        var counterProcesado = Counter(procesado);
        var counterArmaduras = Counter(armaduras);
        var counterCarnes = Counter(carnes);

        var lowestPricePescado = LowestPrice(pescado);
        var lowestPriceVegetales = LowestPrice(vegetales);
        var lowestPriceProcesado = LowestPrice(procesado);
        var lowestPriceArmaduras = LowestPrice(armaduras);
        var lowestPriceCarnes = LowestPrice(carnes);

        pescadoA.text = $"{counterPescado.Item1} - ${lowestPricePescado.Item1}";
        pescadoB.text = $"{counterPescado.Item2} - ${lowestPricePescado.Item2}";
        pescadoC.text = $"{counterPescado.Item3} - ${lowestPricePescado.Item3}";
        espada.text = $"{counterArmaduras.Item1} - ${lowestPriceArmaduras.Item1}";
        escudo.text = $"{counterArmaduras.Item2} - ${lowestPriceArmaduras.Item2}";
        hacha.text = $"{counterArmaduras.Item3} - ${lowestPriceArmaduras.Item3}";
        tarta.text = $"{counterProcesado.Item1} - ${lowestPriceProcesado.Item1}";
        torta.text = $"{counterProcesado.Item2} - ${lowestPriceProcesado.Item2}";
        pan.text = $"{counterProcesado.Item3} - ${lowestPriceProcesado.Item3}";
        galletas.text = $"{counterProcesado.Item4} - ${lowestPriceProcesado.Item4}";
        banana.text = $"{counterVegetales.Item1} - ${lowestPriceVegetales.Item1}";
        zanahoria.text = $"{counterVegetales.Item2} - ${lowestPriceVegetales.Item2}";
        sandia.text = $"{counterVegetales.Item3} - ${lowestPriceVegetales.Item3}";
        carne.text = $"{counterCarnes.Item1} - ${lowestPriceCarnes.Item1}";
        jamon.text = $"{counterCarnes.Item2} - ${lowestPriceCarnes.Item2}";
        chorizo.text = $"{counterCarnes.Item3} - ${lowestPriceCarnes.Item3}";
    }

    (int,int,int,int) Counter(Stand stand)
    {
        var value1 = 0;
        var value2 = 0;
        var value3 = 0;
        var value4 = 0;

        var tupleCounter = stand.tupleList.TupleTypeCounter();
        foreach (var x in tupleCounter)
        {
            if (x.Item1 == "Pescado azul") value1 = x.Count;
            else if (x.Item1 == "Pescado rojo") value2 = x.Count;
            else if (x.Item1 == "Pescado verde") value3 = x.Count;
            else if (x.Item1 == "Espada") value1 = x.Count;
            else if (x.Item1 == "Escudo") value2 = x.Count;
            else if (x.Item1 == "Hacha") value3 = x.Count;
            else if (x.Item1 == "Tarta") value1 = x.Count;
            else if (x.Item1 == "Torta") value2 = x.Count;
            else if (x.Item1 == "Pan") value3 = x.Count;
            else if (x.Item1 == "Galletas") value4 = x.Count;
            else if (x.Item1 == "Banana") value1 = x.Count;
            else if (x.Item1 == "Zanahoria") value2 = x.Count;
            else if (x.Item1 == "Sandía") value3 = x.Count;
            else if (x.Item1 == "Carne") value1 = x.Count;
            else if (x.Item1 == "Jamón") value2 = x.Count;
            else if (x.Item1 == "Chorizo") value3 = x.Count;
        }

        return (value1, value2, value3, value4);
    }

    (int,int,int,int) LowestPrice(Stand stand)
    {
        var tuples = stand.tupleList;

        var value1 = 0;
        var value2 = 0;
        var value3 = 0;
        var value4 = 0;

        foreach (var price in tuples.GetLowestPricePerType())
        {
            if (price.ItemType == "Pescado azul") value1 = price.LowestPrice;
            else if (price.ItemType == "Pescado rojo") value2 = price.LowestPrice;
            else if (price.ItemType == "Pescado verde") value3 = price.LowestPrice;
            else if (price.ItemType == "Espada") value1 = price.LowestPrice;
            else if (price.ItemType == "Escudo") value2 = price.LowestPrice;
            else if (price.ItemType == "Hacha") value3 = price.LowestPrice;
            else if (price.ItemType == "Tarta") value1 = price.LowestPrice;
            else if (price.ItemType == "Torta") value2 = price.LowestPrice;
            else if (price.ItemType == "Pan") value3 = price.LowestPrice;
            else if (price.ItemType == "Galletas") value4 = price.LowestPrice;
            else if (price.ItemType == "Banana") value1 = price.LowestPrice;
            else if (price.ItemType == "Zanahoria") value2 = price.LowestPrice;
            else if (price.ItemType == "Sandía") value3 = price.LowestPrice;
            else if (price.ItemType == "Carne") value1 = price.LowestPrice;
            else if (price.ItemType == "Jamón") value2 = price.LowestPrice;
            else if (price.ItemType == "Chorizo") value3 = price.LowestPrice;
        }

        return (value1, value2, value3, value4);
    }
}
