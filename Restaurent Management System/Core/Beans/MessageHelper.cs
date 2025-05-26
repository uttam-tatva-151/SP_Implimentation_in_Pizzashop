namespace PMSCore.Beans
{
    public class MessageHelper
    {

        /*
            * Operation :- Adding, Deleting, Updating
        */
        #region Success Messages

        public static string GetSuccessMessageForReadOperation(string entity)
        {
            if (string.IsNullOrWhiteSpace(entity))
            {
                return Constants.GENERAL_SUCCESS; // Default/fallback message
            }
            return string.Format(Constants.SUCCESS_READ, entity);
        }
        public static string GetSuccessMessageForAddOperation(string entity)
        {
            if (string.IsNullOrWhiteSpace(entity))
            {
                return Constants.GENERAL_SUCCESS; // Default/fallback message
            }
            return string.Format(Constants.SUCCESS_ADDED, entity);
        }

        public static string GetSuccessMessageForUpdateOperation(string entity)
        {
            if (string.IsNullOrWhiteSpace(entity))
            {
                return Constants.GENERAL_SUCCESS; // Default/fallback message
            }
            return string.Format(Constants.SUCCESS_UPDATED, entity);
        }

        public static string GetSuccessMessageForDeleteOperation(string entity)
        {
            if (string.IsNullOrWhiteSpace(entity))
            {
                return Constants.GENERAL_SUCCESS; // Default/fallback message
            }
            return string.Format(Constants.SUCCESS_DELETED, entity);
        }

        #endregion

        #region Error Messages


        public static string GetErrorMessageForAddOperation(string entity)
        {
            if (string.IsNullOrWhiteSpace(entity))
            {
                return Constants.GENERAL_ERROR;
            }
            return string.Format(Constants.ERROR_ADDING, entity);
        }

        public static string GetErrorMessageForUpdateOperation(string entity)
        {
            if (string.IsNullOrWhiteSpace(entity))
            {
                return Constants.GENERAL_ERROR;
            }
            return string.Format(Constants.ERROR_UPDATING, entity);
        }

        public static string GetErrorMessageForDeleteOperation(string entity)
        {
            if (string.IsNullOrWhiteSpace(entity))
            {
                return Constants.GENERAL_ERROR;
            }
            return string.Format(Constants.ERROR_DELETING, entity);
        }

        public static string GetNotFoundMessage(string entity)
        {
            if (string.IsNullOrWhiteSpace(entity))
            {
                return Constants.GENERAL_ERROR;
            }
            return string.Format(Constants.ERROR_NOT_FOUND, entity);
        }

        #endregion

        #region Warning Messages


        public static string GetWarningMessageForInvalidInput(string entity)
        {
            if (string.IsNullOrWhiteSpace(entity))
            {
                return Constants.WARNING_INVALID_CREDENTIALS;
            }
            return string.Format(Constants.WARNING_INVALID_INPUT, entity);
        }
        public static string GetWarningMessageForAllReadyEntityExists(string entity)
        {
            if (string.IsNullOrWhiteSpace(entity))
            {
                return string.Format(Constants.WARNING_ALL_READY_EXISTS, Constants.DEFAULT_ENTITY);
            }
            return string.Format(Constants.WARNING_ALL_READY_EXISTS, entity);
        }
        public static string GetWarningMessageForDeleteConfirmation(string entity)
        {
            if (string.IsNullOrWhiteSpace(entity))
            {
                return string.Format(Constants.WARNING_DELETE_CONFIRMATION, Constants.DEFAULT_ENTITY);
            }
            return string.Format(Constants.WARNING_DELETE_CONFIRMATION, entity);
        }
        public static string GetWarningMessageForMultipleDeleteConfirmation(string entity)
        {
            if (string.IsNullOrWhiteSpace(entity))
            {
                return string.Format(Constants.WARNING_MULTIPLE_DELETE_CONFIRMATION, Constants.DEFAULT_ENTITY);
            }
            return string.Format(Constants.WARNING_MULTIPLE_DELETE_CONFIRMATION, entity);
        }
        public static string GetWarningMessageForNoSection(string entity)
        {
            if (string.IsNullOrWhiteSpace(entity))
            {
                return string.Format(Constants.WARNING_NOTHING_SELECTED, Constants.DEFAULT_ENTITY);
            }
            return string.Format(Constants.WARNING_NOTHING_SELECTED, entity);
        }

        #endregion

        #region Info Messages

        public static string GetInfoMessageForLoading(string entity)
        {
            if (string.IsNullOrWhiteSpace(entity))
            {
                return string.Format(Constants.INFO_LOADING, Constants.DEFAULT_ENTITY);
            }
            return string.Format(Constants.INFO_LOADING, entity);
        }
        public static string GetInfoMessageForNoRecordsFound(string entity)
        {
            if (string.IsNullOrWhiteSpace(entity))
            {
                return string.Format(Constants.INFO_NO_RECORDS_FOUND, Constants.DEFAULT_ENTITY);
            }
            return string.Format(Constants.INFO_NO_RECORDS_FOUND, entity);
        }
        #endregion
    
        #region  Custom Message 
    
        /* 
            * Tamplate :- "Welcome {0}, your role is {1}."
            * Objects :- "Uttam", "Administrator"

            * Function Calls :- string formattedMessage = MessageHelper.GetCustomMessage(template, "Uttam", "Administrator");

            * Output :- Welcome Uttam, your role is Administrator.
        */
        public static string GetCustomMessage(string template, params object[] args)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(template))
                {
                    return "Message template is missing."; // Handle null or empty template
                }
                return string.Format(template, args);
            }
            catch (FormatException)
            {
                return "An error occurred while formatting the message."; // Handle formatting errors
            }
        }

        #endregion
    }
}
