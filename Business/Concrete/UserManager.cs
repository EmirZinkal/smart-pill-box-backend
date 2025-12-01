using Business.Abstract;
using Business.Constants;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;

namespace Business.Concrete
{
    public class UserManager : IUserService
    {
        // Dependency Injection (Bağımlılık Enjeksiyonu)
        private readonly IUserDal _userDal;

        public UserManager(IUserDal userDal)
        {
            _userDal = userDal;
        }

        public IResult Add(User user)
        {
            // Kullanıcı eklenirken e-posta adresi sistemde mevcut olmamalı.
            var existingUser = _userDal.Get(u => u.Email == user.Email);
            if (existingUser != null)
            {
                // Hata Durumu: Core katmanındaki ErrorResult'ı kullan
                return new ErrorResult(Messages.UserEmailAlreadyExists);
            }

            _userDal.Add(user);
            // Başarı Durumu: Core katmanındaki SuccessResult'ı kullan
            return new SuccessResult(Messages.UserAdded);
        }

        public IResult Delete(User user)
        {
            _userDal.Delete(user);
            return new SuccessResult(Messages.UserDeleted);
        }

        public IDataResult<List<User>> GetAll()
        {
            var users = _userDal.GetList().ToList();
            return new SuccessDataResult<List<User>>(users, Messages.UsersListed);
        }

        public IDataResult<User> GetByEmail(string email)
        {
            var user = _userDal.Get(u => u.Email == email);
            if (user == null)
            {
                return new ErrorDataResult<User>(Messages.UserNotFound);
            }
            return new SuccessDataResult<User>(user, Messages.UserFetched);
        }

        public IDataResult<User> GetById(int userId)
        {
            var user = _userDal.Get(u => u.Id == userId);
            if (user == null)
            {
                return new ErrorDataResult<User>(Messages.UserNotFound);
            }
            return new SuccessDataResult<User>(user, Messages.UserFetched);
        }

        public IResult Update(User user)
        {
            _userDal.Update(user);
            return new SuccessResult(Messages.UserUpdated);
        }
    }
}