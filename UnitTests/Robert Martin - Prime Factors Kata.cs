/*
Robert Martin - Prime Factors Kata
http://www.butunclebob.com/ArticleS.UncleBob.ThePrimeFactorsKata
*/
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DynamicSugar;

namespace DynamicSugarSharp_UnitTests {

    [TestClass]
    public class RobertMartin_PrimeFactorsKata {

        public static List<int> generate_v1(int n) {

            List<int> primes = new List<int>();

            for (int candidate = 2; n > 1; candidate++){

                for ( ; n % candidate == 0; n/=candidate ){

                    primes.Add(candidate);
                }
            }
            return primes;
        }
                
        public static List<int> generate(int n) {

            List<int> primes = new List<int>();
            int candidate    = 2;

            while( n > 1 ){
                while(n % candidate == 0){

                    primes.Add(candidate);
                    n /= candidate; 
                }
                candidate++;
            }
            return primes;
        }
        [TestMethod]
        public void PrimeFactorsKata_UnitTests() {

            DS.ListHelper.AssertListEqual( DS.List(3,3),              generate(9) );
            DS.ListHelper.AssertListEqual( DS.List(2,7),              generate(14) );
            DS.ListHelper.AssertListEqual( DS.List(13),               generate(13) );
            DS.ListHelper.AssertListEqual( DS.List(2, 3),             generate(6) );
            DS.ListHelper.AssertListEqual( DS.List(2, 2, 2, 11),      generate(88) );
            DS.ListHelper.AssertListEqual( DS.List(2, 2, 2, 2, 2, 3), generate(96) );

            //foreach(var i in DSSharp.Range(30000)) { var z = generate(i); }
        }
    }
}

