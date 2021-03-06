﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumaneSociety
{
    public static class Query
    {        
        static HumaneSocietyDataContext db;

        static Query()
        {
            db = new HumaneSocietyDataContext();
        }

        internal static List<USState> GetStates()
        {
            List<USState> allStates = db.USStates.ToList();       

            return allStates;
        }
            
        internal static Client GetClient(string userName, string password)
        {
            Client client = db.Clients.Where(c => c.UserName == userName && c.Password == password).Single();

            return client;
        }

        internal static List<Client> GetClients()
        {
            List<Client> allClients = db.Clients.ToList();

            return allClients;
        }

        internal static void AddNewClient(string firstName, string lastName, string username, string password, string email, string streetAddress, int zipCode, int stateId)
        {
            Client newClient = new Client();

            newClient.FirstName = firstName;
            newClient.LastName = lastName;
            newClient.UserName = username;
            newClient.Password = password;
            newClient.Email = email;

            Address addressFromDb = db.Addresses.Where(a => a.AddressLine1 == streetAddress && a.Zipcode == zipCode && a.USStateId == stateId).FirstOrDefault();

            // if the address isn't found in the Db, create and insert it
            if (addressFromDb == null)
            {
                Address newAddress = new Address();
                newAddress.AddressLine1 = streetAddress;
                newAddress.City = null;
                newAddress.USStateId = stateId;
                newAddress.Zipcode = zipCode;                

                db.Addresses.InsertOnSubmit(newAddress);
                db.SubmitChanges();

                addressFromDb = newAddress;
            }

            // attach AddressId to clientFromDb.AddressId
            newClient.AddressId = addressFromDb.AddressId;

            db.Clients.InsertOnSubmit(newClient);

            db.SubmitChanges();
        }

        internal static void UpdateClient(Client clientWithUpdates)
        {
            // find corresponding Client from Db
            Client clientFromDb = null;

            try
            {
                clientFromDb = db.Clients.Where(c => c.ClientId == clientWithUpdates.ClientId).Single();
            }
            catch(InvalidOperationException e)
            {
                Console.WriteLine("No clients have a ClientId that matches the Client passed in.");
                Console.WriteLine("No update have been made.");
                return;
            }
            
            // update clientFromDb information with the values on clientWithUpdates (aside from address)
            clientFromDb.FirstName = clientWithUpdates.FirstName;
            clientFromDb.LastName = clientWithUpdates.LastName;
            clientFromDb.UserName = clientWithUpdates.UserName;
            clientFromDb.Password = clientWithUpdates.Password;
            clientFromDb.Email = clientWithUpdates.Email;

            // get address object from clientWithUpdates
            Address clientAddress = clientWithUpdates.Address;

            // look for existing Address in Db (null will be returned if the address isn't already in the Db
            Address updatedAddress = db.Addresses.Where(a => a.AddressLine1 == clientAddress.AddressLine1 && a.USStateId == clientAddress.USStateId && a.Zipcode == clientAddress.Zipcode).FirstOrDefault();

            // if the address isn't found in the Db, create and insert it
            if(updatedAddress == null)
            {
                Address newAddress = new Address();
                newAddress.AddressLine1 = clientAddress.AddressLine1;
                newAddress.City = null;
                newAddress.USStateId = clientAddress.USStateId;
                newAddress.Zipcode = clientAddress.Zipcode;                

                db.Addresses.InsertOnSubmit(newAddress);
                db.SubmitChanges();

                updatedAddress = newAddress;
            }

            // attach AddressId to clientFromDb.AddressId
            clientFromDb.AddressId = updatedAddress.AddressId;
            
            // submit changes
            db.SubmitChanges();
        }
        
        internal static void AddUsernameAndPassword(Employee employee)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.EmployeeId == employee.EmployeeId).FirstOrDefault();

            employeeFromDb.UserName = employee.UserName;
            employeeFromDb.Password = employee.Password;

            db.SubmitChanges();
        }

        internal static Employee RetrieveEmployeeUser(string email, int employeeNumber)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.Email == email && e.EmployeeNumber == employeeNumber).FirstOrDefault();

            if (employeeFromDb == null)
            {
                throw new NullReferenceException();
            }
            else
            {
                return employeeFromDb;
            }
        }

        internal static Employee EmployeeLogin(string userName, string password)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.UserName == userName && e.Password == password).FirstOrDefault();

            return employeeFromDb;
        }

        internal static bool CheckEmployeeUserNameExist(string userName)
        {
            Employee employeeWithUserName = db.Employees.Where(e => e.UserName == userName).FirstOrDefault();

            return employeeWithUserName != null;
        }


        //// TODO Items: ////
        
        // TODO: Allow any of the CRUD operations to occur here
        internal static void RunEmployeeQueries(Employee employee, string crudOperation)
        {
            switch (crudOperation)
            {
                case "create":
                    db.Employees.InsertOnSubmit(employee);
                    db.SubmitChanges();
                    break;
                case "read":
                    employee = db.Employees.Where(e => e.EmployeeNumber == employee.EmployeeNumber).Single();                  
                    UserInterface.DisplayEmployeeInfo(employee);
                    break;
                case "update":
                    Employee updateEmployeeFromDb = db.Employees.Where(e => e.EmployeeNumber == employee.EmployeeNumber).Single();
                    updateEmployeeFromDb.FirstName = employee.FirstName;
                    updateEmployeeFromDb.LastName = employee.LastName;
                    updateEmployeeFromDb.EmployeeNumber = employee.EmployeeNumber;
                    updateEmployeeFromDb.Email = employee.Email;
                    db.SubmitChanges();
                    break;
                case "delete":
                    Employee employee1 = db.Employees.Where(e => e.EmployeeNumber == employee.EmployeeNumber && e.LastName == employee.LastName).Single();
                    db.Employees.DeleteOnSubmit(employee1);
                    db.SubmitChanges();
                    break;
            }            
        }

        // TODO: Animal CRUD Operations
        internal static void AddAnimal(Animal animal)//done
        {
            Animal newAnimal = animal;
            db.Animals.InsertOnSubmit(newAnimal);
            db.SubmitChanges();
        }

        internal static Animal GetAnimalByID(int id)//done
        {
            Animal animal = db.Animals.Where(a => a.AnimalId == id).Single();
            return animal;
        }

        internal static void UpdateAnimal(int animalId, Dictionary<int, string> updates)
        {

            for (int i = 0; i < updates.Count; i++)
            {
                Animal animal = db.Animals.Where(a => a.AnimalId == animalId).Single();
                
                switch (updates.ElementAt(i).Key)
                {
                    case 1:
                        animal.CategoryId = Convert.ToInt32(updates.ElementAt(i).Value);
                        break;
                    case 2:
                        animal.Name = updates.ElementAt(i).Value;
                       break;
                    case 3:
                        animal.Age = Convert.ToInt32(updates.ElementAt(i).Value);
                        break;
                    case 4:
                        animal.Demeanor = updates.ElementAt(i).Value;
                        break;
                    case 5:
                        animal.KidFriendly = Convert.ToBoolean(updates.ElementAt(i).Value);
                        break;
                    case 6:
                        animal.PetFriendly = Convert.ToBoolean(updates.ElementAt(i).Value);
                        break;
                    case 7:
                        animal.Weight = Convert.ToInt32(updates.ElementAt(i).Value);
                        break;
                    case 8:
                        animal.AnimalId = Convert.ToInt32(updates.ElementAt(i).Value);
                        break;

                }
                db.SubmitChanges();
               
            }

        }

        internal static void RemoveAnimal(Animal animal)//done
        {
            Animal newAnimal = animal;
            db.Animals.DeleteOnSubmit(animal);
            db.SubmitChanges();
        }
        
        // TODO: Animal Multi-Trait Search
        internal static IQueryable<Animal> SearchForAnimalsByMultipleTraits(Dictionary<int, string> updates) // parameter(s)?
        {
            var animals = db.Animals;
            List<Animal> animalsFromSearch = new List<Animal>();
           
            foreach (var animal in animals)
            {
                animalsFromSearch.Add(animal);
            }
            
            for (int i = 0; i < updates.Count; i++)
            {
                
                switch (updates.ElementAt(i).Key)
                {
                    case 1:
                        var animalsToRemove = db.Animals.Where(a => a.CategoryId != Convert.ToInt32(updates.ElementAt(i).Value));
                        animalsFromSearch = animalsFromSearch.Except(animalsToRemove).ToList();
                        break;
                    case 2:
                        animalsToRemove = db.Animals.Where(a => a.Name != updates.ElementAt(i).Value);
                        animalsFromSearch = animalsFromSearch.Except(animalsToRemove).ToList();
                        break;
                    case 3:
                        animalsToRemove = db.Animals.Where(a => a.Age != Convert.ToInt32(updates.ElementAt(i).Value));
                        animalsFromSearch = animalsFromSearch.Except(animalsToRemove).ToList();
                        break;
                    case 4:
                        animalsToRemove = db.Animals.Where(a => a.Demeanor != updates.ElementAt(i).Value);
                        animalsFromSearch = animalsFromSearch.Except(animalsToRemove).ToList();
                        break;
                    case 5:
                        animalsToRemove = db.Animals.Where(a => a.KidFriendly != Convert.ToBoolean(updates.ElementAt(i).Value));
                        animalsFromSearch = animalsFromSearch.Except(animalsToRemove).ToList();
                        break;
                    case 6:
                        animalsToRemove = db.Animals.Where(a => a.PetFriendly != Convert.ToBoolean(updates.ElementAt(i).Value));
                        animalsFromSearch = animalsFromSearch.Except(animalsToRemove).ToList();
                        break;
                    case 7:
                        animalsToRemove = db.Animals.Where(a => a.Weight != Convert.ToInt32(updates.ElementAt(i).Value));
                        animalsFromSearch = animalsFromSearch.Except(animalsToRemove).ToList();
                        break;
                    case 8:
                        animalsToRemove = db.Animals.Where(a => a.AnimalId != Convert.ToInt32(updates.ElementAt(i).Value));
                        animalsFromSearch = animalsFromSearch.Except(animalsToRemove).ToList();
                        break;

                }
                

            }
            var querable = animalsFromSearch.AsQueryable();
            return querable;
        }
        // TODO: Misc Animal Things
        internal static int GetCategoryId(string categoryName)//done
        {
            Category category = db.Categories.Where(c => c.Name == categoryName).Single();
            return category.CategoryId;
        }
        
        internal static Room GetRoom(int animalId)
        {
            Room room = db.Rooms.Where(r => r.AnimalId == animalId).Single();
            return room;
        }
        
        internal static int GetDietPlanId(string dietPlanName)
        {
            DietPlan dietPlan = db.DietPlans.Where(d => d.Name == dietPlanName).Single();
            return dietPlan.DietPlanId;
        }

        // TODO: Adoption CRUD Operations
        internal static void Adopt(Animal animal, Client client)//done
        {
            Adoption adoption = new Adoption();
            animal.AdoptionStatus = "Pending";
            adoption.Animal = animal;
            adoption.Client = client;
            adoption.ApprovalStatus = "Pending";
            adoption.AdoptionFee = 200;
            db.Adoptions.InsertOnSubmit(adoption);
            db.SubmitChanges();
        }

        internal static IQueryable<Adoption> GetPendingAdoptions()
        {
            IQueryable<Adoption> pendingAdoptions = db.Adoptions.Where(p => p.ApprovalStatus == "Pending");
            return pendingAdoptions;
        }

        internal static void UpdateAdoption(bool isAdopted, Adoption adoption)
        {
            adoption = db.Adoptions.Where(a => a.AnimalId == adoption.AnimalId).Single();
            if(isAdopted == true)
            {
                adoption.ApprovalStatus = "Approved";
                adoption.PaymentCollected = true;

                adoption.Animal.AdoptionStatus = "Adopted";
                db.SubmitChanges();

              
            }
            else
            {
                adoption.Animal.AdoptionStatus = "Available";
                adoption.ApprovalStatus = "Denied";
                adoption.PaymentCollected = false;
                db.SubmitChanges();
            }
        }

        internal static void RemoveAdoption(int animalId, int clientId)
        {
            Adoption adoption = db.Adoptions.Where(a => a.AnimalId == animalId && a.ClientId == clientId).Single();
            db.Adoptions.DeleteOnSubmit(adoption);
            db.SubmitChanges();
        }

        // TODO: Shots Stuff
        internal static IQueryable<AnimalShot> GetShots(Animal animal)
        {
            IQueryable<AnimalShot> animalShots = db.AnimalShots.Where(a => a.AnimalId == animal.AnimalId);
            return animalShots;
        }

        internal static void UpdateShot(string shotName, Animal animal)
        {
            AnimalShot animalShot = new AnimalShot();
            animalShot.AnimalId = animal.AnimalId;
            Shot shot = db.Shots.Where(s => s.Name == shotName).Single();
            animalShot.ShotId = shot.ShotId;
            animalShot.DateReceived = DateTime.Today;
            db.AnimalShots.InsertOnSubmit(animalShot);
        }
    }
}