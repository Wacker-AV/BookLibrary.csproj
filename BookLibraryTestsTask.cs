using System;
using NUnit.Framework;
using FluentAssertions;

namespace BookLibrary
{
    public class BookLibraryTestsTask
    {
        public virtual IBookLibrary CreateBookLibrary() //этот метод удалять нельзя, без него не будет работать
        {
            return new BookLibrary(); //меняется на разные реализации BookLibrary при запуске в системе проверки заданий
        }

        //Позитивный тест. Проверяем, что клиент может встать в очередь за существующей книгой
        [Test]
        public void PositiveTest()
        {
            var bookLibrary = CreateBookLibrary();
            var id = bookLibrary.AddBook(new Book("Книга1"));
            bookLibrary.CheckoutBook(id, "User1");
            bookLibrary.Enqueue(id, "User2");
        }

        //Проверяем, что поле name должно быть заполнено обязательно 
        [Test]
        public void Enqueue_NameNotNull()
        {
            try
            {
                var bookLibrary = CreateBookLibrary();
                Guid nullGuid = new Guid();
                bookLibrary.Enqueue(nullGuid, null);
            }
            catch (ArgumentNullException ex) {
                Assert.AreEqual("Значение не может быть неопределенным.\r\nИмя параметра: userName", ex.Message);
            }
        }

        //Проверяем, что нельзя встать в очередь за несуществующей книгой
        [Test]
        public void Test2()
        {
            var bookLibrary = CreateBookLibrary();
            Guid wrong_id = new Guid();
            try
           {
                bookLibrary.Enqueue(wrong_id, "User1");
            }
            catch (BookLibraryException ex) {
                Assert.AreEqual("Book with bookId '" + wrong_id + "' was not found.", ex.Message);
            }
        }

        //Проверяем что хозяин книги не может встать в очередь за ней
        [Test]
        public void Test3()
        {
            var bookLibrary = CreateBookLibrary();
            var id = bookLibrary.AddBook(new Book("Книга1"));
            String name = "User1";
            bookLibrary.CheckoutBook(id, name);
            try
            {
                bookLibrary.Enqueue(id, name);
            }
            catch (BookLibraryException ex) {
                Assert.AreEqual("Cannot enqueue user '" + name + "' for book 'Книга1' with id '" + id + "', which user holds", ex.Message);
            }
        }

        //Проверяем, что нельзя встать в очередь если у книги нет владельца
        [Test]
        public void Test4()
        {
            var bookLibrary = CreateBookLibrary();
            var id = bookLibrary.AddBook(new Book("Книга1"));
            String name = "User1";
            try
            {
                bookLibrary.Enqueue(id, name);
            }
            catch (BookLibraryException ex)
            {
                Assert.AreEqual("Cannot enqueue if book is free and queue is empty. Checkout book instead.", ex.Message);
            }
        }

        //Проверяем, что нельзя встать в очередь если клиент уже стоит в очереди за этой книгой
        [Test]
        public void Test5()
        {
            var bookLibrary = CreateBookLibrary();
            var id = bookLibrary.AddBook(new Book("Книга1"));
            String user1 = "User1";
            String user2 = "User2";
            String user3 = "User3";
            bookLibrary.CheckoutBook(id, user1);
            bookLibrary.Enqueue(id, user2);
            bookLibrary.Enqueue(id, user3);
            try
            {
                bookLibrary.Enqueue(id, user2);
            }
            catch (BookLibraryException ex)
            {
                Assert.AreEqual("User '" + user2 + "' is already in queue", ex.Message);
            }
        }


    }
	
}