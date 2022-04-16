namespace API.MOFI_2.CustomExceptions {
    public class InvalidIdException : SystemException {
        public InvalidIdException() : base("[ERROR] - Invalid ID provided.") {
        }
    }
}
