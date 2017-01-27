using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.Infrastructure;
using System.Windows;
using System.Data.Entity;

namespace EFTutorials
{
    interface IEntityObjectState
    {
        EntityObjectState ObjectState { get; set; }
    }
    public enum EntityObjectState
    {
        Added, Modified, Deleted, Unchanged
    }
    class Program
    {
        static void Main(string[] args)
        {
            Student student1WithUser1 = null;
            Student student1WithUser2 = null;

            //User 1 gets student
            using (var context = new SchoolDBEntities())
            {
                context.Configuration.ProxyCreationEnabled = false;
                student1WithUser1 = context.Students.Where(s => s.StudentID == 1).Single();
            }

            //User 2 gets the same student
            using (var context = new SchoolDBEntities())
            {
                context.Configuration.ProxyCreationEnabled = false;
                student1WithUser2 = context.Students.Where(s => s.StudentID == 1).Single();
            }
            //User 1 updates Student name
            student1WithUser1.StudentName = "Edited from user1";

            //User 2 updates Student name
            student1WithUser2.StudentName = "Edited from user2";

            using (var context = new SchoolDBEntities())
            {
                try
                {
                    context.Entry(student1WithUser1).State = EntityState.Modified;
                    context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    Console.WriteLine(ex);
                    Console.WriteLine("Optimistic Concurrency exception occured (for User1)");
                }
            }

            //User 2 saves changes after User 1. 
            //User 2 will get concurrency exection 
            //because CreateOrModifiedDate is different in the database 
            using (var context = new SchoolDBEntities())
            {
                try
                {
                    context.Entry(student1WithUser2).State = EntityState.Modified;
                    context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    Console.WriteLine(ex);
                    Console.WriteLine("Optimistic Concurrency exception occured (for User2)");
                }
            }

            SpaceSection();
            Console.WriteLine("Press any key");
            Console.ReadKey();
        }

        private static void SpaceSection()
        {
            Console.WriteLine("");
            Console.WriteLine("---------------------------------------");
            Console.WriteLine("");
        }
    }
}
