using System;
using System.Security.Cryptography.X509Certificates;

class FormulaManager
{
    public string formula;
    public FormulaManager() 
    {
        this.formula = "";
    }
        // if there is a letter or a () with numbers right next to them, we have to add an *.
        // if there is no x nor y, notify it to the user.

        // radical = () ** 1/indice
        // potencia = **
        // multiplicacion = *
        // suma = +
        // resta = -
        // division = /
        // seno = sin()
        // coseno = cos()
        // tangente = tan().

    public void FormulaProcessor(string character)
    {
        formula += character;
    }
}