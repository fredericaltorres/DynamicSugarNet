using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DynamicSugar;
using System.Dynamic;

namespace DynamicSugarSharp_UnitTests {

    //TODO:Try extension method to List<T>

    [TestClass]
    public class List_EM_UnitTests {

        [TestMethod]
        public void ProjectEuler_Problem1() {

            var r = DS.Range(10)
                .Filter(x => ((x % 3 == 0) || (x % 5 == 0)))
                    .Inject((x, v) => v += x);
            Assert.AreEqual(23, r);

            var r2 = DS.Range(10).AsQueryable()
                .Where(x => ((x % 3 == 0) || (x % 5 == 0)))
                    .Aggregate((x, v) => x + v);
            Assert.AreEqual(23, r2);

            var r3 = (from xx in DS.Range(10).AsQueryable()
                      where ((xx % 3 == 0) || (xx % 5 == 0))
                      select xx).Aggregate((x, v) => x + v);
            Assert.AreEqual(23, r3);
        }
        [TestMethod]
        public void Range_3() {

            var i = 0;
            foreach (var r in DS.Range(3)) {
                Assert.AreEqual(i, r);
                i++;
            }
        }
        [TestMethod]
        public void Range_103() {

            var i = 0;
            foreach (var r in DS.Range(103)) {
                Assert.AreEqual(i, r);
                i++;
            }
        }
        [TestMethod]
        public void Range_100_Step2() {

            var i = 0;
            foreach (var r in DS.Range(100, 2)) {
                Assert.AreEqual(i, r);
                i += 2;
            }
        }
        [TestMethod]
        public void Range_100_Start10_Step10() {
            
            DS.Assert.AreEqual(
                DS.List(10,20,30,40,50,60,70,80,90), 
                DS.Range(10,100,10)
            );
        }


        [TestMethod]
        public void List_OfDifferentTypes() {

            var l = DS.List<object>(1, "a", 3.0);
                       
            Assert.AreEqual(1  , l[0]);
            Assert.AreEqual("a", l[1]);
            Assert.AreEqual(3.0, l[2]);
        }

        [TestMethod]
        public void IsEmpty() {

            //Assert.IsTrue(DS.List<int>().IsEmpty());            
            Assert.IsFalse(DS.List(1, 2, 3).IsEmpty());            
        }
        [TestMethod]
        public void IsNullOrEmpty() {

            List<int> nullList = null;
            Assert.IsTrue(nullList.IsNullOrEmpty());            
            //Assert.IsTrue(DS.List<int>().IsNullOrEmpty());            
            Assert.IsFalse(DS.List(1, 2, 3).IsNullOrEmpty());            
        }


        [TestMethod]
        public void Array_Integer() {

            var l = DS.Array(1, 2, 3);
            Assert.AreEqual(1, l[0]);
            Assert.AreEqual(2, l[1]);
            Assert.AreEqual(3, l[2]);
        }


        [TestMethod]
        public void Queue_Integer() {

            var l = DS.Queue(1, 2, 3);
            Assert.AreEqual(1, l.Dequeue());
            Assert.AreEqual(2, l.Dequeue());
            Assert.AreEqual(3, l.Dequeue());
        }

         [TestMethod]
        public void Stack_Integer() {

            var l = DS.Stack(1, 2, 3);
            Assert.AreEqual(3, l.Pop());
            Assert.AreEqual(2, l.Pop());
            Assert.AreEqual(1, l.Pop());
        }

        [TestMethod]
        public void List_Integer() {

            var l = DS.List(1, 2, 3);
            Assert.AreEqual(1, l[0]);
            Assert.AreEqual(2, l[1]);
            Assert.AreEqual(3, l[2]);
        }
        [TestMethod]
        public void List_String() {

            var l = DS.List("1", "2", "3");
            Assert.AreEqual("1", l[0]);
            Assert.AreEqual("2", l[1]);
            Assert.AreEqual("3", l[2]);
        }
        [TestMethod]
        public void Identical_String() {

            Assert.IsTrue(DS.ListHelper.Identical(DS.List("1"), DS.List("1")));
            Assert.IsTrue(DS.ListHelper.Identical(DS.List("1", "2", "3"), DS.List("1", "2", "3")));
            Assert.IsTrue(DS.ListHelper.Identical(DS.List("1", "2", "3", null), DS.List("1", "2", "3", null)));

            Assert.IsFalse(DS.ListHelper.Identical(DS.List("1", "2", "3"), DS.List("1", "2", "4")));
            Assert.IsFalse(DS.ListHelper.Identical(DS.List("1", "2", "3"), DS.List("1", "2", "3", "4")));

            Assert.IsTrue(DS.List("1", "2", "3").Identical(DS.List("1", "2", "3")));
        }
        [TestMethod]
        public void Identical_Double() {

            Assert.IsTrue(DS.ListHelper.Identical(DS.List(1.1, 2.2, 3.3), DS.List(1.1, 2.2, 3.3)));
            Assert.IsFalse(DS.ListHelper.Identical(DS.List(1.1, 2.2, 3.3), DS.List(1.1, 2.2, 3.4)));
            Assert.IsTrue(DS.List(1.1, 2.2, 3.3).Identical(DS.List(1.1, 2.2, 3.3)));
        }
        [TestMethod]
        public void Map_WithLambdaExpression() {
                        
            DS.Assert.AreEqual(
                DS.List(1, 4, 9), 
                DS.List(1, 2, 3).Map(e => e * e)
            );
        }
        [TestMethod]
        public void Map_WithLambdaExpression_WithLocalVariable() {

            int MyConst = 2;
            DS.Assert.AreEqual(
                DS.List(2, 4, 6), 
                DS.List(1, 2, 3).Map(e => e * MyConst)
            );                  
        }
        [TestMethod]
        public void Map_WithBlockStatment() {
            
            DS.Assert.AreEqual(
                DS.List(2, 4, 6), 
                DS.List(1, 2, 3).Map(e => { return e * 2; })
            );
        }
        [TestMethod]
        public void Map_WithLambdaExpression_String() {

            string MyConst = "Hi ";
            DS.Assert.AreEqual(
                DS.List("Hi fred", "Hi joe", "Hi diane"), 
                DS.List("fred", "joe", "diane").Map(e => MyConst + e)
            );
        }
        [TestMethod]
        public void Format_Integer() {
                        
            Assert.AreEqual("1, 2, 3", DS.List(1, 2, 3).Format());            
        }
        [TestMethod]
        public void Format_Boolean() {
            
            Assert.AreEqual("True, False, True", DS.ListHelper.Format(DS.List(true, false, true)));
        }
        [TestMethod]
        public void Filter() {
                
            DS.Assert.AreEqual(
                DS.List(0, 2, 4, 6, 8), 
                DS.List(0, 1, 2, 3, 4, 5, 6, 7, 8, 9).Filter( e => e % 2 == 0)
            );
        }
        [TestMethod]
        public void Reject() {
                        
            DS.Assert.AreEqual(
                DS.List(1,3,5,7,9),
                DS.Range(10).Reject( e => e % 2 == 0)
            );
        }
        [TestMethod]
        public void Select_StandardNET() {

            var l = DS.List(0, 1, 2, 3, 4, 5, 6, 7, 8, 9).Select(e => e % 2 == 0).ToList();
            var r = DS.List(true, false, true, false, true, false, true, false, true, false);
            DS.Assert.AreEqual(r, l);
        }
        [TestMethod]
        public void Filter_Decimal() {

            DS.Assert.AreEqual(
                DS.List(0M, 2M, 4M, 6M, 8M), 
                DS.List(0M, 1M, 2M, 3M, 4M, 5M, 6M, 7M, 8M, 9M).Filter(e => e % 2 == 0)
            );
        }
        [TestMethod]
        public void Inject() {

            Assert.AreEqual(10, DS.ListHelper.Inject(DS.List(1, 2, 3, 4), (v, e) => v += e));            
            Assert.AreEqual(10, DS.List(1, 2, 3, 4).Inject((v, e) => v += e));
            Assert.AreEqual(10, DS.List(1, 2, 3, 4).Aggregate((v, e) => v += e));

            var s = DS.List("1", "2", "3", "4").Aggregate(
                (v, e) =>
                    v += e
                );
            Assert.AreEqual("1234", s);
        }
        [TestMethod]
        public void Reduce() {

            Assert.AreEqual(10, DS.List(1, 2, 3, 4).Reduce((v, e) => v += e));
        }
        [TestMethod]
        public void ForEach_StandardNet() {

            int z = 0;

            DS.List(0, 1, 2, 3).ForEach(
                i => { z += i; }
            );
            Assert.AreEqual(6, z);
        }
        [TestMethod]
        public void Clone_EmptyList() {

            var l = new List<int>();
            DS.Assert.AreEqual(l, DS.ListHelper.Clone(l));            
        }
        [TestMethod]
        public void Clone() {
            
            var refList = DS.List(1,2,3,4);
            DS.Assert.AreEqual(refList, refList.Clone());
        }
        [TestMethod]
        public void Intersection() {

            var l1    = DS.List(1, 2, 3, 4);
            var l2    = DS.List(3, 4, 5, 6);
            var inter = DS.List(3, 4);
            DS.Assert.AreEqual(inter, l1.Intersection(l2));
            DS.Assert.AreEqual(inter, l1.Intersect(l2).ToList());
            

        }
        [TestMethod]
        public void Add() {

            var l1  = DS.List(1, 2, 3);
            var l2  = DS.List(3, 4, 5);
            var sum = DS.List(1, 2, 3, 3, 4, 5);
            DS.Assert.AreEqual(sum, DS.ListHelper.Add(l1, l2));
            
        }
        [TestMethod]
        public void Add_EmptyList() {

            var l1  = DS.List(1, 2, 3);
            var l2  = new List<int>();
            var sum = DS.List(1, 2, 3);
            DS.Assert.AreEqual(sum, DS.ListHelper.Add(l1, l2));
        }
        [TestMethod]
        public void Add_2EmptyList() {

            var l1  = new List<int>();
            var l2  = new List<int>();
            var sum = new List<int>();
            DS.Assert.AreEqual(sum, DS.ListHelper.Add(l1, l2));
        }
        [TestMethod]
        public void Substract() {

            var l1  = DS.List(1, 2, 3);
            var l2  = DS.List(3, 4, 5);
            var sum = DS.List(1, 2);
            DS.Assert.AreEqual(sum, DS.ListHelper.Substract(l1, l2));
            DS.Assert.AreEqual(sum, l1.Substract(l2));
        }
        [TestMethod]
        public void Substract_String() {

            var l1  = DS.List("1", "2", "3");
            var l2  = DS.List("3", "4", "5");
            var sum = DS.List("1", "2");

            DS.Assert.AreEqual(sum, DS.ListHelper.Substract(l1, l2));
            DS.Assert.AreEqual(sum, l1.Substract(l2));
        }
        [TestMethod]
        public void Merge_Int() {
            
            DS.Assert.AreEqual(
                DS.List(1, 2, 3, 4, 5),
                DS.ListHelper.Merge(
                    DS.List(1, 2, 3), 
                    DS.List(3, 4, 5)
                )
            );
        }
        [TestMethod]
        public void Merge_Unique() {

            DS.Assert.AreEqual(
                DS.List(1, 2, 3, 4, 5),                
                DS.List(1, 2, 3, 4).Merge(DS.List(4, 4, 5), true)
            );           
        }
        [TestMethod]
        public void Merge_NonUnique() {

            DS.Assert.AreEqual(
                DS.List(1, 2, 3, 4, 4, 4, 5),
                DS.ListHelper.Merge(
                    DS.List(1, 2, 3, 4), 
                    DS.List(4, 4, 5), false
                )
            );           
        }
      
        [TestMethod]
        public void Rest_Int() {

            DS.Assert.AreEqual(
                DS.List(2,3,4,5),
                DS.List(1,2,3,4,5).Rest()
            );
        }
        [TestMethod]
        public void First_Int() {

            Assert.AreEqual(1, DS.List(1, 2, 3, 4, 5).First());
        }
        [TestMethod]
        public void First_String() {

            Assert.AreEqual("1", DS.List("1", "2").First());
        }
        [TestMethod]
        public void Last_Int() {

            Assert.AreEqual(5, DS.List(1, 2, 3, 4, 5).Last());
        }
        [TestMethod]
        public void Last_String() {

            Assert.AreEqual("2", DS.List("1", "2").Last());
        }
        [TestMethod]
        public void Find_LINQ_Standard() {

            Assert.AreEqual(5, DS.List(1, 2, 3, 4, 5).Find(e => e == 5));
        }
        [TestMethod]
        public void Find_Instance_LINQ_Standard() {

            var people   = GetPeopleList();
            var firstOld = people.Find(e => e.Age >= 40);
            Assert.IsTrue(firstOld.LastName == "Montesquieu");
        }
        public static List<Person> GetPeopleList() {

            var people = DS.List(
                new Person() { LastName = "Descartes",   FirstName = "Rene",   Age = 20 },
                new Person() { LastName = "Montesquieu", FirstName = "Gerard", Age = 40 },
                new Person() { LastName = "Rousseau",    FirstName = "JJ",     Age = 60 }
            );
            return people;
        }
        [TestMethod]
        public void Distinct_LINQ_Standard() {

            DS.Assert.AreEqual(
                DS.List(1, 2, 3, 4, 5),
                DS.List(1, 2, 3, 4, 5, 1, 2, 3).Distinct().ToList()
            );
        }
        [TestMethod]
        public void Pluck_Instance_Property_Integer() {

            var people = GetPeopleList();

            DS.Assert.AreEqual(
                DS.List(20, 40, 60),
                people.Pluck<int, Person>("Age")
            );
        }
        [TestMethod]
        public void Pluck_Instance_Property() {

            var people = GetPeopleList();
            DS.Assert.AreEqual(
                DS.List("Descartes", "Montesquieu", "Rousseau"),
                people.Pluck<string, Person>("LastName")
            );
        }
        [TestMethod]
        public void Pluck_Instance_Function() {

            var people = GetPeopleList();
            DS.Assert.AreEqual(
                DS.List("#Descartes.Rene.0001", "#Montesquieu.Gerard.0001", "#Rousseau.JJ.0001"),
                people.Pluck<string, Person>("GetUniqueID()")
            );
        }
        [TestMethod]
        public void Max_LINQ_Standard() {

            Assert.AreEqual(5, DS.List(1, 2, 3, 4, 5).Max());
            var v = DS.List("aa", "a", "aaa").Max();
            Assert.AreEqual("aaa", v);
        }
        [TestMethod]
        public void Min_LINQ_Standard() {

            Assert.AreEqual(1, DS.List(1, 2, 3, 4, 5).Min());
            var v = DS.List("aa", "a", "aaa").Min();
            Assert.AreEqual("a", v);
        }
        [TestMethod]
        public void IndexOf_LINQ_Standard() {

            var l = DS.List("a", "b", "c");
            Assert.AreEqual(1, l.IndexOf(l[1]));
            Assert.AreEqual(-1, l.IndexOf("z"));
        }
        [TestMethod]
        public void All_LINQ_Standard() {

            var l = DS.List(1, 2, 3, 4, 5);            
            Assert.IsTrue(l.All( e => e < 10));
            Assert.IsFalse(l.All(e => e > 1));
        }
        [TestMethod]
        public void Any_LINQ_Standard() {

            var l = DS.List(1, 2, 3, 4, 5);
            Assert.IsTrue(l.Any( e => e>=5));
            Assert.IsFalse(l.All(e => e>=6));
        }
        [TestMethod]
        public void Include_OneValue() {
                
            Assert.IsTrue ( DS.Range(10).Include(5) );            
            Assert.IsFalse( DS.Range(10).Include(11) );            
        }
        [TestMethod]
        public void Include_Values() {
                            
            Assert.IsTrue( DS.Range(10).Include(DS.List(1, 2, 3, 4, 5)));
            Assert.IsTrue( DS.Range(10).Include(1, 2, 3, 4, 5));
        }       
        [TestMethod]
        public void Sort() {

            var l = DS.List(3,2,4,6,5,1);
            l.Sort();            
            DS.Assert.AreEqual(
                DS.List(1,2,3,4,5,6),
                l
            );
        }
        [TestMethod]
        public void Without() {

            var l = DS.Range(10);

            DS.Assert.AreEqual(
                DS.List(0,1,2,3,4,5),
                l.Without(DS.List(6,7,8,9))
            );
        }
        [TestMethod]
        public void In() {

            var l = DS.Range(5);
            int i = 1;
            Assert.IsTrue(i.In(l));
            Assert.IsTrue(i.In(1,2,3,4,5));            
            i     = 11;
            Assert.IsFalse(i.In(l));            
            Assert.IsFalse(i.In(1,2,3,4,5));
        }
        //[TestMethod]
        public void expando() {

            dynamic o   = new ExpandoObject();
            o.LastName  = "TORRES";
            o["a"]      = 1;
            var i       = o.a;
            var s       = o.LastName;
        }
        [TestMethod]
        public void ToFile_FromFile() {

            var l        = DS.Range(10);
            var fileName = String.Format( @"{0}\DSSharpLibrary_UnitTests.txt", Environment.GetEnvironmentVariable("TEMP") );
            DeleteFile(fileName);

            l.ToFile(fileName, true);
            Assert.IsTrue(System.IO.File.Exists(fileName));
                       
            var expected = new List<string>();
            foreach(var i in l) expected.Add(i.ToString());

            var l1 = DS.ListHelper.FromFile<string>(fileName);
            DS.Assert.AreEqual(expected, l1);

            var l2 = DS.ListHelper.FromFile<int>(fileName);
            DS.Assert.AreEqual(l, l2);
        }

        private static void DeleteFile(string fileName) {

            if (System.IO.File.Exists(fileName))
                System.IO.File.Delete(fileName);
        }
    }
}

