using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Siesa.SDK.Frontend.Utils;
using Microsoft.CodeAnalysis.Scripting;

namespace Siesa.SDK.Frontend.Tests
{
    public class EvaluatorTests
    {
        [Fact]
        public async Task TestEvaluateCodeWithRoslyn()
        {
            // Arrange
            string code = "3 + 4";
            object globals = null;
            bool useRoslyn = true;

            // Act
            var result = await Evaluator.EvaluateCode(code, globals, useRoslyn: useRoslyn);

            // Assert
            Assert.Equal(7, result);
        }

        [Fact]
        public async Task TestEvaluateCodeWithReflection()
        {
            // Arrange
            string code = "MyMethod(3, 4)";
            object globals = new MyClass();
            bool useRoslyn = false;

            // Act
            var result = await Evaluator.EvaluateCode(code, globals, useRoslyn: useRoslyn);

            // Assert
            Assert.Equal(7, result);
        }

        [Fact]
        public async Task TestEvaluateCodeWithInvalidCodeFormat()
        {
            // Arrange
            string code = "3 +";
            object globals = null;
            bool useRoslyn = true;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(async () => await Evaluator.EvaluateCode(code, globals, useRoslyn: useRoslyn));
        }

        [Fact]
        public async Task TestEvaluateCodeWithMethodNotFound()
        {
            // Arrange
            string code = "NonExistentMethod(3, 4)";
            object globals = new MyClass();
            bool useRoslyn = false;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(async () => await Evaluator.EvaluateCode(code, globals, useRoslyn: useRoslyn));
        }

        [Fact]
        public async Task TestEvaluateCodeWithEmptyArguments()
        {
            // Arrange
            string code = "MyMethod()";
            object globals = new MyClass();
            bool useRoslyn = false;

            // Assert
            await Assert.ThrowsAsync<ArgumentException>(async () => await Evaluator.EvaluateCode(code, globals, useRoslyn: useRoslyn));
        }

        [Fact]
        public async Task TestEvaluateCodeWithStaticArguments()
        {
            // Arrange
            string code = "MyConcat(\"Hello\", \"World\")";
            object globals = new MyClass();
            bool useRoslyn = false;

            // Act
            var result = await Evaluator.EvaluateCode(code, globals, useRoslyn: useRoslyn);

            // Assert
            Assert.Equal("Hello-_-World", result);
        }

        [Fact]
        public async Task TestEvaluateCodeWithStaticArgumentsRoslyn()
        {
            // Arrange
            string code = "MyConcat(\"Hello\",\"World\")";
            object globals = new MyClass();
            bool useRoslyn = true;

            // Act
            var result = await Evaluator.EvaluateCode(code, globals, useRoslyn: useRoslyn);

            // Assert
            Assert.Equal("Hello-_-World", result);
        }

        [Fact]
        public async Task TestEvaluateCodeWithDynamicArguments()
        {
            // Arrange
            string code = "MyConcat(\"Hello\")";
            object globals = new MyClass() { OtherProperty = "World" };
            bool useRoslyn = false;

            // Act
            var result = await Evaluator.EvaluateCode(code, globals, useRoslyn: useRoslyn);

            // Assert
            Assert.Equal("My Other Property: World -_- Hello", result);
        }

        [Fact]
        public async Task TestEvaluateCodeWithDynamicArgumentsRoslyn()
        {
            // Arrange
            string code = "MyConcat(\"Hello\")";
            object globals = new MyClass() { OtherProperty = "World" };
            bool useRoslyn = true;

            // Act
            var result = await Evaluator.EvaluateCode(code, globals, useRoslyn: useRoslyn);

            // Assert
            Assert.Equal("My Other Property: World -_- Hello", result);
        }

        [Fact]
        public async Task TestEvaluateCodeWithInternalProperty()
        {
            // Arrange
            string code = "MyProperty";
            object globals = new MyClass() { MyProperty = "Hello World" };
            bool useRoslyn = false;

            // Act
            var result = await Evaluator.EvaluateCode(code, globals, useRoslyn: useRoslyn);

            // Assert
            Assert.Equal("Hello World", result);
        }

        [Fact]
        public async Task TestEvaluateCodeWithInternalPropertyRoslyn()
        {
            // Arrange
            string code = "MyProperty";
            object globals = new MyClass() { MyProperty = "Hello World" };
            bool useRoslyn = true;

            // Act
            var result = await Evaluator.EvaluateCode(code, globals, useRoslyn: useRoslyn);

            // Assert
            Assert.Equal("Hello World", result);
        }

        [Fact]
        public async Task TestEvaluateCodeWithInternalPropertyInt()
        {
            // Arrange
            string code = "MyIntProperty";
            object globals = new MyClass() { MyIntProperty = 123 };
            bool useRoslyn = false;

            // Act
            var result = await Evaluator.EvaluateCode(code, globals, useRoslyn: useRoslyn);

            // Assert
            Assert.Equal(123, result);
        }

        [Fact]
        public async Task TestEvaluateCodeWithInternalPropertyIntRoslyn()
        {
            // Arrange
            string code = "MyIntProperty";
            object globals = new MyClass() { MyIntProperty = 123 };
            bool useRoslyn = true;

            // Act
            var result = await Evaluator.EvaluateCode(code, globals, useRoslyn: useRoslyn);

            // Assert
            Assert.Equal(123, result);
        }

        [Fact]
        public async Task TestEvaluateCodeWithInternalPropertyBool()
        {
            // Arrange
            string code = "MyBoolProperty";
            object globals = new MyClass() { MyBoolProperty = true };
            bool useRoslyn = false;

            // Act
            var result = await Evaluator.EvaluateCode(code, globals, useRoslyn: useRoslyn);

            // Assert
            Assert.Equal(true, result);
        }

        [Fact]
        public async Task TestEvaluateCodeWithInternalPropertyBoolRoslyn()
        {
            // Arrange
            string code = "MyBoolProperty";
            object globals = new MyClass() { MyBoolProperty = true };
            bool useRoslyn = true;

            // Act
            var result = await Evaluator.EvaluateCode(code, globals, useRoslyn: useRoslyn);

            // Assert
            Assert.Equal(true, result);
        }

        [Fact]
        public async Task TestEvaluateCodeWithInternalPropertyDateTime()
        {
            // Arrange
            string code = "MyDateTimeProperty";
            object globals = new MyClass() { MyDateTimeProperty = new DateTime(2018, 1, 1) };
            bool useRoslyn = false;

            // Act
            var result = await Evaluator.EvaluateCode(code, globals, useRoslyn: useRoslyn);

            // Assert
            Assert.Equal(new DateTime(2018, 1, 1), result);
        }

        [Fact]
        public async Task TestEvaluateCodeWithInternalPropertyDateTimeRoslyn()
        {
            // Arrange
            string code = "MyDateTimeProperty";
            object globals = new MyClass() { MyDateTimeProperty = new DateTime(2018, 1, 1) };
            bool useRoslyn = true;

            // Act
            var result = await Evaluator.EvaluateCode(code, globals, useRoslyn: useRoslyn);

            // Assert
            Assert.Equal(new DateTime(2018, 1, 1), result);
        }

        [Fact]
        public async Task TestEvaluateCodeWithInternalPropertyList()
        {
            // Arrange
            string code = "MyListProperty";
            object globals = new MyClass() { MyListProperty = new List<string>() { "Hello", "World" } };
            bool useRoslyn = false;

            // Act
            var result = await Evaluator.EvaluateCode(code, globals, useRoslyn: useRoslyn);

            // Assert
            Assert.Equal(new List<string>() { "Hello", "World" }, result);
        }

        [Fact]
        public async Task TestEvaluateCodeWithInternalPropertyListRoslyn()
        {
            // Arrange
            string code = "MyListProperty";
            object globals = new MyClass() { MyListProperty = new List<string>() { "Hello", "World" } };
            bool useRoslyn = true;

            // Act
            var result = await Evaluator.EvaluateCode(code, globals, useRoslyn: useRoslyn);

            // Assert
            Assert.Equal(new List<string>() { "Hello", "World" }, result);
        }

        [Fact]
        public async Task TestEvaluateCodeWithWaitingAsyncMethod()
        {
            // Arrange
            string code = "await MyAsyncMethod(\"Hello World\")";
            object globals = new MyClass();
            bool useRoslyn = false;

            // Act
            var result = await Evaluator.EvaluateCode(code, globals, useRoslyn: useRoslyn);

            // Assert
            Assert.Equal("Hello World", result);
        }

        [Fact]
        public async Task TestEvaluateCodeWithWaitingAsyncMethodRoslyn()
        {
            // Arrange
            string code = "await MyAsyncMethod(\"Hello World\")";
            object globals = new MyClass();
            bool useRoslyn = true;

            // Act
            var result = await Evaluator.EvaluateCode(code, globals, useRoslyn: useRoslyn);

            // Assert
            Assert.Equal("Hello World", result);
        }

        [Fact]
        public async Task TestEvaluateCodeWithAsyncMethod()
        {
            // Arrange
            string code = "MyAsyncMethod(\"Hello World\")";
            object globals = new MyClass();
            bool useRoslyn = false;

            // Act
            var result = await Evaluator.EvaluateCode(code, globals, useRoslyn: useRoslyn);

            // Assert
            Assert.Equal(TaskStatus.RanToCompletion, ((Task)result).Status);
        }

        [Fact]
        public async Task TestEvaluateCodeWithAsyncMethodRoslyn()
        {
            // Arrange
            string code = "MyAsyncMethod(\"Hello World\")";
            object globals = new MyClass();
            bool useRoslyn = true;

            // Act
            var result = await Evaluator.EvaluateCode(code, globals, useRoslyn: useRoslyn);

            // Assert TaskStatus.RanToCompletion
            Assert.Equal(TaskStatus.RanToCompletion, ((Task)result).Status);
        }

        private async Task TestDynamicMethod(bool useRoslyn)
        {
            MyClass globals = new MyClass();
            globals.MyListProperty = new List<string>() { "Hello", "World" };
            globals.MyDateTimeProperty = new DateTime(2018, 1, 1);
            globals.MyBoolProperty = true;
            globals.MyIntProperty = 123;

            globals.Parent = new MyClass();
            globals.Parent.MyListProperty = new List<string>() { "Hello", "World", "From", "Parent" };
            globals.Parent.MyDateTimeProperty = new DateTime(2018, 1, 2);
            globals.Parent.MyBoolProperty = false;
            globals.Parent.MyIntProperty = 456;
            
            globals.Parent.Parent = new MyClass();
            globals.Parent.Parent.MyListProperty = new List<string>() { "Hello", "World", "From", "Parent", "Parent" };
            globals.Parent.Parent.MyDateTimeProperty = new DateTime(2018, 1, 3);
            globals.Parent.Parent.MyBoolProperty = true;
            globals.Parent.Parent.MyIntProperty = 789;

            Dictionary<string, object> testCases = new Dictionary<string, object>();
            testCases.Add("MyDynamicMethod(\"Hello World\")", "Hello World");
            testCases.Add("MyDynamicMethod('H')", 'H');
            testCases.Add("MyDynamicMethod(5)", 5);
            testCases.Add("MyDynamicMethod(5.5)", 5.5);
            testCases.Add("MyDynamicMethod(true)", true);
            testCases.Add("MyDynamicMethod(false)", false);
            testCases.Add("MyDynamicMethod(MyListProperty)", new List<string>() { "Hello", "World" });
            testCases.Add("MyDynamicMethod(MyDateTimeProperty)", new DateTime(2018, 1, 1));
            testCases.Add("MyDynamicMethod(MyBoolProperty)", true);
            testCases.Add("MyDynamicMethod(MyIntProperty)", 123);
            testCases.Add("MyDynamicMethod(null)", null);
            testCases.Add("MyDynamicMethod(Parent)", globals.Parent);
            testCases.Add("MyDynamicMethod(Parent.MyListProperty)", new List<string>() { "Hello", "World", "From", "Parent" });
            testCases.Add("MyDynamicMethod(Parent.MyDateTimeProperty)", new DateTime(2018, 1, 2));
            testCases.Add("MyDynamicMethod(Parent.MyBoolProperty)", false);
            testCases.Add("MyDynamicMethod(Parent.MyIntProperty)", 456);
            testCases.Add("MyDynamicMethod(Parent.Parent)", globals.Parent.Parent);
            testCases.Add("MyDynamicMethod(Parent.Parent.MyListProperty)", new List<string>() { "Hello", "World", "From", "Parent", "Parent" });
            testCases.Add("MyDynamicMethod(Parent.Parent.MyDateTimeProperty)", new DateTime(2018, 1, 3));
            testCases.Add("MyDynamicMethod(Parent.Parent.MyBoolProperty)", true);
            testCases.Add("MyDynamicMethod(Parent.Parent.MyIntProperty)", 789);
            testCases.Add("MyMethod", globals.MyMethod);
            testCases.Add("MyAsyncMethod", globals.MyAsyncMethod);
            testCases.Add("MyVoidMethod", globals.MyVoidMethod);
            testCases.Add("Parent.MyBoolProperty", globals.Parent.MyBoolProperty);


            foreach (var testCase in testCases)
            {
                // Arrange
                string code = testCase.Key;
                

                // Act
                var result = await Evaluator.EvaluateCode(code, globals, useRoslyn: useRoslyn);

                // Assert
                Assert.Equal(testCase.Value, result);
            }
        }

        [Fact]
        public async Task TestEvaluateCodeWithDynamicMethod()
        {
            await TestDynamicMethod(false);
        }

        [Fact]
        public async Task TestEvaluateCodeWithDynamicMethodRoslyn()
        {
            await TestDynamicMethod(true);
        }

        public class MyClass
        {
            public string MyProperty { get; set; }
            public string OtherProperty { get; set; }
            public int MyIntProperty { get; set; }
            public bool MyBoolProperty { get; set; }
            public DateTime MyDateTimeProperty { get; set; }
            public List<string> MyListProperty { get; set; }

            public MyClass Parent { get; set; }


            public int MyMethod(int a, int b)
            {
                return a + b;
            }

            public string MyConcat(string a, string b)
            {
                return $"{a}-_-{b}";
            }

            public string MyConcat(string a)
            {
                return $"My Other Property: {OtherProperty} -_- {a}";
            }

            public async Task<string> MyAsyncMethod(string a)
            {
                return await Task.FromResult(a);
            }

            public dynamic MyDynamicMethod(dynamic a)
            {
                return a;
            }

            public void MyVoidMethod()
            {
                MyProperty = "Hello World";
            }
        }
    }
}
