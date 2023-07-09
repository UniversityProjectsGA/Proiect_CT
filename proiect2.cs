using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;

class Program
{
    static void Main(string[] args)
    {
        int k = 2;
        // Gramatica 1
        var gramatica = new Dictionary<string, List<string>>();
        /*gramatica["S"] = new List<string> { "aBDh" };
        gramatica["B"] = new List<string> { "cC" };
        gramatica["C"] = new List<string> { "bC", "ε" };
        gramatica["D"] = new List<string> { "EF" };
        gramatica["E"] = new List<string> { "g", "ε" };
        gramatica["F"] = new List<string> { "f", "ε" };*/

        gramatica["S"] = new List<string> { "AAABB" };
        gramatica["A"] = new List<string> { "a" , "ε" };
        gramatica["B"] = new List<string> { "b", "ε" };

        // Gramatica 2
        var gramatica2 = new Dictionary<string, List<string>>();
        gramatica2["S"] = new List<string> { "S+T", "T" };

        // Gramatica 3
        var gramatica3 = new Dictionary<string, List<string>>();
        /*gramatica3["S"] = new List<string> { "iEtSeS", "iEtS", "a" };
        gramatica3["E"] = new List<string> { "b" };*/

        gramatica3["S"] = new List<string> { "aA", "aAB", "aC" };
        gramatica3["A"] = new List<string> { "b", "bA" };
        gramatica3["B"] = new List<string> { "b", "ε" };
        gramatica3["C"] = new List<string> { "c" };

        // Afisare gramatica initiala
        Console.WriteLine("k = " + k);
        Console.WriteLine("Gramatica 1:");
        foreach (var neterminal in gramatica.Keys)
        {
            Console.WriteLine(neterminal + " -> " + string.Join("|", gramatica[neterminal]));
        }

        // a) calculeaza multimile FirstK(X), FollowK(X)
        var multimiFirstK = new Dictionary<string, HashSet<string>>();
        Console.WriteLine("Multimi FirstK:");
        foreach (var neterminal in gramatica.Keys)
        {
            multimiFirstK[neterminal] = MultimiFirstK(gramatica, neterminal, k);
            Console.WriteLine(neterminal + " -> " + string.Join("|", multimiFirstK[neterminal]));
        }

        var multimiFollowK = new Dictionary<string, HashSet<string>>();
        Console.WriteLine("Multimi FollowK:");
        foreach (var neterminal in gramatica.Keys)
        {
            multimiFollowK[neterminal] = MultimiFollowK(gramatica, neterminal, k, multimiFirstK);
            Console.WriteLine(neterminal + " -> " + string.Join("|", multimiFollowK[neterminal]));
        }

        // b) elimina recursivitatea la stanga

        // Afisare gramatica initiala
        Console.WriteLine("Gramatica 2:");
        foreach (var neterminal in gramatica2.Keys)
        {
            Console.WriteLine(neterminal + " -> " + string.Join("|", gramatica2[neterminal]));
        }

        foreach (var neterminal in gramatica2.Keys)
        {
            var gramaticaFRecSt = EliminareRecursivitateStanga(gramatica2, neterminal);
            Console.WriteLine("Gramatica cu recursivitate la stanga eliminata pentru " + neterminal + ":");
            foreach (var neter in gramaticaFRecSt.Keys)
            {
                Console.WriteLine(neter + " -> " + string.Join("|", gramaticaFRecSt[neter]));
            }
        }

        // c) factorizează stânga gramatica

        // Afisare gramatica initiala
        Console.WriteLine("Gramatica 2:");
        foreach (var neterminal in gramatica3.Keys)
        {
            Console.WriteLine(neterminal + " -> " + string.Join("|", gramatica3[neterminal]));
        }

        var gramaticaFactSt = FactorizareStanga(gramatica3, k);
        Console.WriteLine("Gramatica factorizata stanga:");
        foreach (var neterminal in gramaticaFactSt.Keys)
        {
            Console.WriteLine(neterminal + " -> " + string.Join("|", gramaticaFactSt[neterminal]));
        }
    }

    // Calcul multimi FirstK(X)
    static HashSet<string> MultimiFirstK(Dictionary<string, List<string>> gramatica, string neterminal, int k)
    {
        var multimeFirstK = new HashSet<string>();
        var netermParcurs = new HashSet<string>();
        CalculMultimeFirstK(gramatica, neterminal, k, multimeFirstK, netermParcurs);
        return multimeFirstK;
    }

    static void CalculMultimeFirstK(Dictionary<string, List<string>> gramatica, string neterminal, int k, HashSet<string> multimeFirstK, HashSet<string> netermParcurs)
    {
        // Daca K = 0 multimea FirstK nu este calculata
        if (k == 0) return;

        // Verificam daca neterminalul se afla in gramatica
        if (gramatica.ContainsKey(neterminal))
        {
            // Folosim netermParcurs pentru a memora fiecare neterminal
            netermParcurs.Add(neterminal);
            foreach (var productie in gramatica[neterminal])
            {
                foreach (var charProd in productie)
                {
                    // Trecem printe toate productile si fiecare simbol ale productiilor

                    // Daca simbolul este neterminal si nu a fost inca parcurs
                    if (char.IsUpper(charProd) && !netermParcurs.Contains(charProd.ToString()))
                    {
                        // Apelam functia recursiv pentru a calcula multimea FirstK pentru fiecare simbol
                        CalculMultimeFirstK(gramatica, charProd.ToString(), k - 1, multimeFirstK, netermParcurs);
                    }
                    else
                    {
                        // Adaugam simbolurile in multimile FirstK si iesim din loop 
                        multimeFirstK.Add(charProd.ToString());
                        break;
                    }
                }
            }
            // Stergem neterminalul din netermParcurs
            netermParcurs.Remove(neterminal);
        }
    }
    // Sfarsit calcul multimi FirstK(X)

    // Calcul multimi FollowK(X)
    static HashSet<string> MultimiFollowK(Dictionary<string, List<string>> gramatica, string neterminal, int k, Dictionary<string, HashSet<string>> multimiFirstK)
    {
        var multimeFollowK = new HashSet<string>();
        var simbParcurs = new HashSet<string>();

        // Adaugam $ pentru neterminalul S
        if (neterminal == "S")
            multimeFollowK.Add("$");

        CalculMultimeFollowK(gramatica, neterminal, k, multimeFollowK, simbParcurs, multimiFirstK);
        return multimeFollowK;
    }

    static void CalculMultimeFollowK(Dictionary<string, List<string>> gramatica, string simbol, int k, HashSet<string> multimeFollowK, HashSet<string> simbParcurs, Dictionary<string, HashSet<string>> multimiFirstK)
    {
        // Verificam daca simbolul a fost deja parcurs
        if (simbParcurs.Contains(simbol)) return;

        // Folosim simbParcurs pentru a memora fiecare simbol
        simbParcurs.Add(simbol);

        foreach (var neterminal in gramatica.Keys)
        {
            foreach (var productie in gramatica[neterminal])
            {
                for (int i = 0; i < productie.Length; i++)
                {
                    // Trecem printe toate simbolurile alea fiecarei productile din toate neterminalele din gramatica

                    if (productie[i].ToString() == simbol)
                    {
                        // Verificam daca simbolul din productie[i] este acelasi cu simbolul pe care il procesam 
                        if (i < productie.Length - 1)
                        {
                            // Verificam ca mai exista un alt simbol in productie si il memoram in simbolulUrmator
                            var simbolulUrmator = productie[i + 1].ToString();
                            if (char.IsUpper(simbolulUrmator[0]))
                            {
                                // Testam simbolulUrmator pentru a vedea daca acesta este neterminal
                                var multimeFirstK = new HashSet<string>(multimiFirstK[simbolulUrmator]);
                                // Calculam multimea FirstK pentru simbolulUrmator si verificam daca acesta contine ε
                                if (multimeFirstK.Contains("ε"))
                                {
                                    multimeFirstK.Remove("ε");
                                    // Daca multimea FirstK contine ε acesta este sters
                                    multimeFollowK.UnionWith(multimeFirstK);
                                    // Adaugam la multimea FollowK a simbolului curent multimea FirstK a simbolulUrmator si apelam functia recursiv
                                    CalculMultimeFollowK(gramatica, neterminal, k - 1, multimeFollowK, simbParcurs, multimiFirstK);
                                }
                                else
                                {
                                    // Daca ε nu exista in multimea FirstK a simbolulUrmator adaugam la multimea FollowK a simbolului curent multimea FirstK
                                    multimeFollowK.UnionWith(multimeFirstK);
                                }
                            }
                            else
                            {
                                // Daca simbolulUrmator este terminal il adaugam in multimea FollowK a simbolului curent
                                multimeFollowK.Add(simbolulUrmator);
                            }
                        }
                        else if (neterminal != simbol)
                        {
                            // Apelam functia recursiv daca ultimul simbol din productie este diferit de neterminal din gramatica
                            CalculMultimeFollowK(gramatica, neterminal, k - 1, multimeFollowK, simbParcurs, multimiFirstK);
                        }
                    }
                }
            }
        }
        // Stergem simbolul din simbParcurs
        simbParcurs.Remove(simbol);
    }
    // Sfarsit calcul multimi FollowK(X)

    // Eliminare recursivitate stanga
    static Dictionary<string, List<string>> EliminareRecursivitateStanga(Dictionary<string, List<string>> gramatica, string neterminal)
    {
        // Verificam ca neterminalul exista si are productii
        if (!gramatica.ContainsKey(neterminal) || gramatica[neterminal].Count == 0)
        {
            return gramatica;
        }

        // Variabile pentru a stoca modificari
        var gramaticaFRecSt = new Dictionary<string, List<string>>();
        var productiiNeRecursiveStanga = new List<string>();
        var productiiRecursiveStanga = new List<string>();

        // Impartim productiile din gramatica dupa neterminal, productiile care incep cu neterminal in productiiRecursiveStanga si restul in productiiNeRecursiveStanga
        foreach (var productie in gramatica[neterminal])
        {
            if (productie.StartsWith(neterminal))
            {
                productiiRecursiveStanga.Add(productie);
            }
            else
            {
                productiiNeRecursiveStanga.Add(productie);
            }
        }

        // Verificam daca exista productii in neterminal
        if (productiiRecursiveStanga.Count > 0)
        {
            // Se creaza un neterminal nou
            var neterminalNou = neterminal + "'";
            gramaticaFRecSt[neterminalNou] = new List<string> { "ε" };

            // Impartim productiile in doua productii noi recursive si nerecursive

            // La productiile noi nerecursive adaugam noul neterminal creat
            var productiiNeRecursiveStangaNoi = new List<string>();
            foreach (var productie in productiiNeRecursiveStanga)
            {
                productiiNeRecursiveStangaNoi.Add(productie + neterminalNou);
            }

            // La productiile noi recursive eliminam recursivitatea la stanga si adaugam noul neterminal creat si ε
            var productiiRecursiveStangaNoi = new List<string>();
            foreach (var productie in productiiRecursiveStanga)
            {
                productiiRecursiveStangaNoi.Add(productie.Substring(neterminal.Length) + neterminalNou);
            }
            productiiRecursiveStangaNoi.Add("ε");

            // Cele doua productii noi create sunt adaugate in gramatica
            gramaticaFRecSt[neterminal] = productiiNeRecursiveStangaNoi;
            gramaticaFRecSt[neterminalNou] = productiiRecursiveStangaNoi;
            // Apelem recursiv functia pentru a elimina recursivitatea in neterminalul nou
            gramaticaFRecSt = EliminareRecursivitateStanga(gramaticaFRecSt, neterminalNou);
        }
        else
        {
            gramaticaFRecSt[neterminal] = gramatica[neterminal];
        }

        foreach (var neter in gramatica.Keys)
        {
            if (neter != neterminal)
            {
                // Trecem prin fiecare neterminal din gramatica originala care nu a fost deja procesat
                var productiiNoi = new List<string>();
                foreach (var productie in gramatica[neter])
                {
                    // Verificam daca productia incepe sau nu cu neterminalul procesat deja
                    if (productie.StartsWith(neterminal))
                    {
                        // Daca incepe cu neterminalul procesat deja acesta este schimbat cu gramatica noua calculata anterior
                        foreach (var productieNoua in gramaticaFRecSt[neterminal])
                        {
                            productiiNoi.Add(productieNoua + productie.Substring(neterminal.Length));
                        }
                    }
                    else
                    {
                        // Daca nu incepe cu neterminalul procesat deja acesta este neschimbat
                        productiiNoi.Add(productie);
                    }
                }
                // Adaugam modificarile facute
                gramaticaFRecSt[neter] = productiiNoi;
            }
        }
        // Returnam noua gramatica fara recursivitate la stanga
        return gramaticaFRecSt;
    }
    // Sfarsit eliminare recursivitate stanga

    // Factorizare stânga gramatica
    static Dictionary<string, List<string>> FactorizareStanga(Dictionary<string, List<string>> gramatica, int k)
    {
        // Variablila pentru a stoca modificarile gramaticii
        var gramaticaFactSt = new Dictionary<string, List<string>>();

        // Trecem prin neterminalele din gramatica
        foreach (var neterminal in gramatica.Keys)
        {
            // O variabila pentru toate productiile din neterminal
            var productii = gramatica[neterminal];

            // Se creaza o variabila pentru productiile noi 
            var ProductiiNoi = new List<string>();
            var factor = "";

            // Trecem prin toate productiile
            while (productii.Count > 0)
            {
                // Gasim cel mai lung prefix pentru productii
                factor = CelMaiLungPrefix(productii);
                // Daca prefixul este cel putin egal cu k atunci putem factoriza
                if (factor.Length >= k)
                {
                    // Se creaza un neterminal si o productie noua
                    var neterminalNou = neterminal + "'";
                    var productieNoua = factor + neterminalNou;

                    // Adaugam la ProductiiNoi productia nou creata si stergem prefixul din fiecare productie
                    ProductiiNoi.Add(productieNoua);
                    productii = productii.Select(p => p.Substring(factor.Length)).ToList();
                }
                else
                {
                    // Daca prefixul nu este destul de lung, adaugam productiile fara sa le modificam si le stergem 
                    ProductiiNoi.Add(productii[0]);
                    productii.RemoveAt(0);
                }
            }

            // Adaugam productiile la gramatica
            gramaticaFactSt[neterminal] = ProductiiNoi;

            if (ProductiiNoi.Count > 1)
            {
                // Daca exista ProductiiNoi cream un neterminal nou pentru sufix-urile ramase
                var neterminalNou = neterminal + "'";
                // Adaugam productiile in neterminalul nou al gramaticii
                gramaticaFactSt[neterminalNou] = new List<string>();
                gramaticaFactSt[neterminalNou].Add(factor + neterminalNou);
                gramaticaFactSt[neterminalNou].Add("ε");
            }
        }
        // Returnam noua gramatica factorizata la stanga
        return gramaticaFactSt;
    }

    static string CelMaiLungPrefix(List<string> productii)
    {
        // Daca exista doar un string atunci el este cel mai lung
        if (productii.Count == 1)
        {
            return productii[0];
        }

        // Gasim stingul cu cea mai mica lungime
        var lungimeMinima = productii.Min(s => s.Length);
        var prefix = "";
        // Trecem limitat prin fiecare string din productii
        for (int i = 0; i < lungimeMinima; i++)
        {
            var charProd = productii[0][i];

            // Daca celelate stringuri au caractere diferite la pozitia i, iesim din loop
            if (productii.Any(s => s[i] != charProd))
            {
                break;
            }
            // Adaugam caracterul la prefix
            prefix += charProd;
        }
        // Returnam prefixul
        return prefix;
    }
    // Sfarsit factorizare stânga gramatica
}