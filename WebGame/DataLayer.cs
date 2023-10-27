using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System.Data;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;

/// File: DataLayer.cs
/// Name: Nicholas Tanguay
/// Class: CITC 1317
/// Semester: Fall 2023
/// Project: WebGame Midterm
namespace WebGame
{
    /// <summary>
    /// The data DataLayer class is used to hide implementation details of
    /// connecting to the database doing standard CRUD operations.
    /// 
    /// IMPORTANT NOTES:
    /// On the serverside, any input-output operations should be done asynchronously. This includes
    /// file and database operations. In doing so, the thread is freed up for the entire time a request
    /// is in flight. When a request executes the await code, the request thread is returned back to the
    /// thread pool. When the request is satisfied, the thread is taken from the thread pool and continues.
    /// This is all built into the .NET Core Framework making it very easy to implement into our code.
    /// 
    /// When throwing an exception from an ASYNC function the exception is never thrown back to the calling entity. 
    /// This makes sense because the function could possibly block and cause strange and unexpected 
    /// behavior. Instead, we will LOG the exception.
    /// </summary>
    internal class DataLayer
    {

        #region "Properties"

        /// <summary>
        /// This variable holds the connection details
        /// such as name of database server, database name, username, and password.
        /// The ? makes the property nullable
        /// </summary>
        private readonly string? connectionString = null;

        #endregion

        #region "Constructors"

        /// <summary>
        /// This is the default constructor and has the default 
        /// connection string specified 
        /// </summary>
        public DataLayer()
        {
            //preprocessor directives can help by using a debug build database environment for testing
            // while using a production database environment for production build.
#if (DEBUG)
            //connectionString = @"Data Source=(localdb)\ProjectModels;Initial Catalog=WebWidget;Integrated Security=True;Connect Timeout=30";
            connectionString = @"Server=localhost;Port=3306;Database=WebGame;Uid=root;Pwd=''";
#else
            connectionString = @"Production Server Connection Information";
#endif
        }

        /// <summary>
        /// Parameterized Constructor passing in a connection string
        /// </summary>
        /// <param name="connectionString"></param>
        public DataLayer(string connectionString)
        {
            this.connectionString = connectionString;
        }

        #endregion

        #region "Database Operations"

        /// <summary>
        /// Get all widgets in the database and return in a List
        /// </summary>
        /// <param>None</param>
        /// <returns>List of Widgets </returns>
        /// <exception cref="Exception"></exception>
        public List<Game> GetGames()
        {
            List<Game> games = new();

            try
            {

                //using guarentees the release of resources at the end of scope 
                using MySqlConnection conn = new(connectionString);

                // open the database connection
                conn.Open();

                // create a command object identifying the stored procedure
                using MySqlCommand cmd = new MySqlCommand("spGetGames", conn);

                // set the command object so it knows to execute a stored procedure
                cmd.CommandType = CommandType.StoredProcedure;

                // execute the command which returns a data reader object
                // usually we should use ExecuteReaderAsync() but for this example we will use ExecuteReader()
                using MySqlDataReader rdr = (MySqlDataReader)cmd.ExecuteReader();

                // if the reader contains a data set, convert to widget objects
                while (rdr.Read())
                {
                    Game game = new Game();

                    game.Id = (int)rdr.GetValue(0);
                    game.Title = (string)rdr.GetValue(1);
                    game.Description = (string)rdr.GetValue(2);
                    game.Rating = (int)rdr.GetValue(3);
                    game.Price = (double)rdr.GetValue(4);

                    games.Add(game);
                }
            }
            catch (Exception)
            {
                //normally we would write to a log here
                //rethrow exception
                throw;
            }
            finally
            {
                // no clean up because the 'using' statements guarantees closing resources
            }

            //check for widgets length to be zero after returned from database
            return games;

        } // end function GetWidgets

        /// <summary>
        /// Get a user by key (GUID)
        /// returns a single User object or a null User
        /// </summary>
        /// <param name="key"></param>
        /// <returns>Widget</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="Exception"></exception>
        public Game? GetUserLevelByKey(string key)
        {

            Game? userDTO = null;

            try
            {
                if (key == null)
                {
                    throw new ArgumentNullException("Username or Password can not be null.");
                }

                //using guarentees the release of resources at the end of scope 
                using MySqlConnection conn = new MySqlConnection(connectionString);

                // open the database connection
                conn.Open();

                // create a command object identifying the stored procedure
                using MySqlCommand cmd = new MySqlCommand("spGetUserLevel", conn);

                // set the command object so it knows to execute a stored procedure
                cmd.CommandType = CommandType.StoredProcedure;

                // add parameters to command, which will be passed to the stored procedure
                cmd.Parameters.Add(new MySqlParameter("aUserKey", key));

                // execute the command which returns a data reader object
                using MySqlDataReader rdr = (MySqlDataReader)cmd.ExecuteReader();

                // if the reader contains a data set, load a local user object
                if (rdr.Read())
                {
                    userDTO = new();
                    userDTO.Title = (string)rdr.GetValue(0);
                    userDTO.Id = (int)rdr.GetValue(1);
                }
            }
            catch (ArgumentNullException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                // no clean up because the 'using' statements guarantees closing resources
            }

            return userDTO;

        } // end function GetUserLevelByKey

        /// <summary>
        /// Gets a widget in the database by widget ID and returns an Widget or null
        /// </summary>
        /// <param>Id</param>
        /// <returns>Widget</returns>
        /// <exception cref="Exception"></exception>
        public Game? GetGameById(int Id)
        {
            Game? game = null;

            try
            {

                //using guarentees the release of resources at the end of scope 
                using MySqlConnection conn = new(connectionString);

                // open the database connection
                conn.Open();

                // create a command object identifying the stored procedure
                using MySqlCommand cmd = new MySqlCommand("spGetAGame", conn);

                // set the command object so it knows to execute a stored procedure
                cmd.CommandType = CommandType.StoredProcedure;

                // add parameters to command, which will be passed to the stored procedure
                cmd.Parameters.Add(new MySqlParameter("aid", Id));

                // execute the command which returns a data reader object
                // usually we should use ExecuteReaderAsync() but for this example we will use ExecuteReader()
                using MySqlDataReader rdr = (MySqlDataReader)cmd.ExecuteReader();

                // if the reader contains a data set, convert to widget objects
                if (rdr.Read())
                {
                    //widget is null so create a new instance
                    game = new Game();

                    game.Id = (int)rdr.GetValue(2);
                    game.Title = (string)rdr.GetValue(3);
                    game.Description = (string)rdr.GetValue(4);
                    game.Rating = (int)rdr.GetValue(5);
                    game.Price = (double)rdr.GetValue(6);

                }
            }
            catch (Exception)
            {
                //normally we would write to a log here
                //rethrow exception
                throw;
            }
            finally
            {
                // no clean up because the 'using' statements guarantees closing resources
            }

            //check for widgets length to be zero after returned from database
            return game;

        } // end function GetWidgetById

        /// <summary>
        /// Insert an widget into the database and return the widgetvwith the new ID
        /// </summary>
        /// <param>Widget</param>
        /// <returns>Widget</returns>
        /// <exception cref="Exception"></exception>
        /// <exception cref="ArgumentNullException"></exception>"
        public Game? InsertGame(Game game)
        {

            Game? tempGame = null;
            try
            {
                if (game == null)
                {
                    throw new ArgumentNullException("Game can not be null.");
                }

                //using guarentees the release of resources at the end of scope 
                using MySqlConnection conn = new(connectionString);

                // open the database connection
                conn.Open();

                // create a command object identifying the stored procedure
                using MySqlCommand cmd = new MySqlCommand("spInsertWidget", conn);

                // set the command object so it knows to execute a stored procedure
                cmd.CommandType = CommandType.StoredProcedure;

                // add parameters to command, which will be passed to the stored procedure
                cmd.Parameters.Add(new MySqlParameter("gId", game.Id));
                cmd.Parameters.Add(new MySqlParameter("gTitle", game.Title));
                cmd.Parameters.Add(new MySqlParameter("gDescription", game.Description));
                cmd.Parameters.Add(new MySqlParameter("gRating", game.Rating));
                cmd.Parameters.Add(new MySqlParameter("gPrice", game.Price));

                //create a parameter to hold the output value
                MySqlParameter IdValue = new MySqlParameter("aid", SqlDbType.Int);
                IdValue.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(IdValue);

                // execute the command which returns a data reader object
                // usually we should use ExecuteReaderAsync() but for this example we will use ExecuteReader()
                int count = cmd.ExecuteNonQuery();

                // if the reader contains a data set, convert to widget objects
                if (count > 0)
                {
                    //widget is null so create a new instance
                    tempGame = new Game();

                    tempGame.Id = (int)IdValue.Value;

                    tempGame.Id = game.Id;
                    tempGame.Title = game.Title;
                    tempGame.Description = game.Description;
                    tempGame.Rating = game.Rating;
                    tempGame.Price = game.Price;

                }
            }
            catch (ArgumentNullException)
            {
                throw;
            }
            catch (Exception)
            {
                //normally we would write to a log here
                //rethrow exception
                throw;
            }
            finally
            {
                // no clean up because the 'using' statements guarantees closing resources
            }

            //widget is null if there was an error
            return tempGame;

        } // end function GetWidgetById

        /// <summary>
        /// Update an widget in the database and return row count affected
        /// </summary>
        /// <param>Id, Widget</param>
        /// <returns>int</returns>
        /// <exception cref="Exception"></exception>
        public int UpdateGame(int Id, Game game)
        {
            int count;

            try
            {
                if (game == null)
                {
                    throw new ArgumentNullException("Game can not be null.");
                }

                //using guarentees the release of resources at the end of scope 
                using MySqlConnection conn = new(connectionString);

                // open the database connection
                conn.Open();

                // create a command object identifying the stored procedure
                using MySqlCommand cmd = new MySqlCommand("spUpdateGame", conn);

                // set the command object so it knows to execute a stored procedure
                cmd.CommandType = CommandType.StoredProcedure;

                // add parameters to command, which will be passed to the stored procedure
                cmd.Parameters.Add(new MySqlParameter("gid", Id));
                cmd.Parameters.Add(new MySqlParameter("gtitle", game.Title));
                cmd.Parameters.Add(new MySqlParameter("gdescription", game.Description));
                cmd.Parameters.Add(new MySqlParameter("grating", game.Rating));
                cmd.Parameters.Add(new MySqlParameter("gprice", game.Price));

                // execute the command which returns a data reader object
                // usually we should use ExecuteReaderAsync() but for this example we will use ExecuteReader()
                count = cmd.ExecuteNonQuery();

            }
            catch (ArgumentNullException)
            {
                throw;
            }
            catch (Exception)
            {
                //normally we would write to a log here
                //rethrow exception
                throw;
            }
            finally
            {
                // no clean up because the 'using' statements guarantees closing resources
            }

            //widget is null if there was an error
            return count;

        } // end function GetWidgetById

        /// <summary>
        /// Delete an widget in the database and return row count affected
        /// </summary>
        /// <param>Id</param>
        /// <returns>int</returns>
        /// <exception cref="Exception"></exception>
        public int DeleteGame(int Id)
        {
            int count;
            try
            {
                //using guarentees the release of resources at the end of scope 
                using MySqlConnection conn = new(connectionString);

                // open the database connection
                conn.Open();

                // create a command object identifying the stored procedure
                using MySqlCommand cmd = new MySqlCommand("spDeleteGame", conn);

                // set the command object so it knows to execute a stored procedure
                cmd.CommandType = CommandType.StoredProcedure;

                // add parameters to command, which will be passed to the stored procedure
                cmd.Parameters.Add(new MySqlParameter("aid", Id));

                // execute the command which returns a data reader object
                // usually we should use ExecuteReaderAsync() but for this example we will use ExecuteReader()
                count = cmd.ExecuteNonQuery();
            }
            catch (Exception)
            {
                //normally we would write to a log here
                //rethrow exception
                throw;
            }
            finally
            {
                // no clean up because the 'using' statements guarantees closing resources
            }

            //widget is null if there was an error
            return count;

        } // end function GetWidgetById

        #endregion

    } // end class DataLayer

} // end namespace