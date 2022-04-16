namespace API.MOFI_2.CustomExceptions {
    public class DefaultUnknownErrorException : SystemException {
        public DefaultUnknownErrorException() : base("[ERROR] - An unexpected error occured!") {
        }
    }
}
