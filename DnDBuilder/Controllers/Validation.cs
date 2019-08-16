using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Mono.Data.Sqlite;
using Newtonsoft.Json.Linq;

namespace DnDBuilder.Controllers
{
    public class Validation
    {
        private const String dbName = "DnDBuilder.sqlite";

        private String errorMsg = null;
        private DataStructure inputData;

        public Validation() { }

        public void setInputData(DataStructure inputData) {
            this.inputData = inputData;
        }

        public String run(Races races, Classes classes, Boolean isCreate) {
            errorMsg = checkName(inputData.name, isCreate);
            if (errorMsg != null)
            {
                return errorMsg;
            }

            errorMsg = checkAge();
            if (errorMsg != null)
            {
                return errorMsg;
            }

            errorMsg = checkGender();
            if (errorMsg != null)
            {
                return errorMsg;
            }

            errorMsg = checkBio();
            if (errorMsg != null)
            {
                return errorMsg;
            }

            errorMsg = checkRace(races);
            if (errorMsg != null)
            {
                return errorMsg;
            }

            errorMsg = checkClass(classes);
            if (errorMsg != null)
            {
                return errorMsg;
            }

            errorMsg = checkLevel();
            if (errorMsg != null)
            {
                return errorMsg;
            }

            errorMsg = checkABPoint();
            if (errorMsg != null)
            {
                return errorMsg;
            }

            return null;
        }

        //Check whether the name is duplicated
        public String checkName(String name, Boolean isCreate) {
            try
            {
                if (name.Equals("")) {
                    return "Character name should not be empty!";
                }
                using (SqliteConnection m_dbConn = new SqliteConnection("Data Source="+ dbName + ";Version=3;")) 
                {
                    m_dbConn.Open();
                    SqliteCommand checkDb = new SqliteCommand("SELECT count(*) FROM characters WHERE name='" + name + "'",m_dbConn);
                    int count = Convert.ToInt32(checkDb.ExecuteScalar());
                    m_dbConn.Close();
                    if (count != 0)
                    {
                        if (isCreate) {
                            return "Charater Name is already exist!";
                        }

                    }
                    else
                    {
                        if (!isCreate) {
                            return "Charater does not exist!";
                        }
                    }

                    return null;

                }
            }
            catch (Exception e) when (e is SqliteException || e is InvalidCastException)
            {
                return e.Message;
            }
        }

        //Check whether the age is between 1 and 500
        private String checkAge()
        {
            if ((inputData.age > 500) || (inputData.age < 1)) {
                return "age should be between 1 and 500";
            }
            return null;
        }

        //Check whether gender 
        private String checkGender()
        {
            if (inputData.gender.Equals(""))
            {
                return "Gender should not be empty!";
            }
            return null;
        }

        //Check whether biography contain less than 500 characters
        private String checkBio()
        {
            if (inputData.bio.Equals(""))
            {
                return "biography should not be empty!";
            }
            if (inputData.bio.Length > 500) {
                return "biography should contain no more than 500 characters.";
            }
            return null;
        }

        //Check Whether the selected race is exist
        private String checkRace(Races races)
        {
            foreach(var result in races.results) {
                if (result.name.Equals(inputData.races))
                {
                    return null;
                }
            }
            return "The selected race does not exist!";
        }

        //Check Whether the selected class is exist
        private String checkClass(Classes classes)
        {
            foreach (var result in classes.results)
            {
                if (result.name.Equals(inputData.classes))
                {
                    return null;
                }
            }
            return "The selected class does not exist!";
        }

        //Check whether the level is between 1 and 20
        private String checkLevel()
        {
            if ((inputData.level < 0) || (inputData.level > 20)) {
                return "Level should be between 1 and 20.";
            }
            return null;
        }

        //Check whether the ability point is correct
        private String checkABPoint()
        {
            int userABSum = 0;

            foreach(int abScore in inputData.userAB)
            {
                if ((abScore < 0) || (abScore > 20)) {
                    return "Certain ability point is input incorrectly";
                }
                else 
                {
                    userABSum += abScore;
                }
            }

            if ((userABSum > 20)||(userABSum < 0)) {
                return "The total ability score is incorrect!";
            }
            return null;
        }


        public String getErrorMsg() {
            return errorMsg;
        }
    }
}
