
{{- for namespace in usingNamespaces -}}
using {{namespace}};
{{~end~}}

namespace {{namespace}}
{
	public partial class {{className}}: Mock<{{targetName}}>
	{
		public {{constructorName}}() : base(MockBehavior.Strict)
		{ }

		public static {{className}} Create()
		{
			return new {{className}}();
		}
		{{~for method in methods~}}
		{{~if method.hasResult~}}

		public {{className}} Setup{{method.fullName}}(
			Func<{{method.resultType}}> result{{if method.parameters.size > 0}},{{end}}
			{{-for parameter in method.parameters}}
			Func<{{parameter.type}}, bool> is{{parameter.originalName.pascalCase}} = null{{if !for.last}},{{end}}
			{{-end}})
		{
			Setup(
					x => x.{{method.fullName}}(
					{{~for parameter in method.parameters~}}
						It.Is<{{parameter.type}}>(fromMock => is{{parameter.originalName.pascalCase}} == null || is{{parameter.originalName.pascalCase}}(fromMock)){{if !for.last}},{{end}}
					{{~end~}}
					)
				)
				.Returns{{if method.isTask}}Async{{end}}(result)
				.Verifiable();

			return this;
		}

		public {{className}} Setup{{method.name}}Sequence{{method.genericArguments}}(
			IEnumerable<Func<{{method.resultType}}>> results{{if method.parameters.size > 0}},{{end}}
			{{-for parameter in method.parameters}}
			Func<{{parameter.type}}, bool> is{{parameter.originalName.pascalCase}} = null{{if !for.last}},{{end}}
			{{-end}})
		{
			var sequenceSetup =
				SetupSequence(
					x => x.{{method.fullName}}(
					{{~for parameter in method.parameters~}}
						It.Is<{{parameter.type}}>(fromMock => is{{parameter.originalName.pascalCase}} == null || is{{parameter.originalName.pascalCase}}(fromMock)){{if !for.last}},{{end}}
					{{~end~}}
					)
				);

			foreach (var result in results)
			{
				sequenceSetup.Returns{{if method.isTask}}Async{{end}}(result);
			}

			return this;
		}
		{{~else if !method.hasResult && method.isTask~}}

		public {{className}} Setup{{method.fullName}}(
			Func<{{method.returnType}}> result = null{{if method.parameters.size > 0}},{{end}}
			{{-for parameter in method.parameters}}
			Func<{{parameter.type}}, bool> is{{parameter.originalName.pascalCase}} = null{{if !for.last}},{{end}}
			{{-end}})
		{
			Setup(
					x => x.{{method.fullName}}(
					{{~for parameter in method.parameters~}}
						It.Is<{{parameter.type}}>(fromMock => is{{parameter.originalName.pascalCase}} == null || is{{parameter.originalName.pascalCase}}(fromMock)){{if !for.last}},{{end}}
					{{~end~}}
					)
				)
				.Returns(result?.Invoke() ?? {{if method.returnType == 'Task'}}Task.FromResult(0){{else}}ValueTask.CompletedTask{{end}})
				.Verifiable();

			return this;
		}

		public {{className}} Setup{{method.name}}Sequence{{method.genericArguments}}(
			IEnumerable<Func<{{method.returnType}}>> results{{if method.parameters.size > 0}},{{end}}
			{{-for parameter in method.parameters}}
			Func<{{parameter.type}}, bool> is{{parameter.originalName.pascalCase}} = null{{if !for.last}},{{end}}
			{{-end}})
		{
			var sequenceSetup =
				SetupSequence(
					x => x.{{method.fullName}}(
					{{~for parameter in method.parameters~}}
						It.Is<{{parameter.type}}>(fromMock => is{{parameter.originalName.pascalCase}} == null || is{{parameter.originalName.pascalCase}}(fromMock)){{if !for.last}},{{end}}
					{{~end~}}
					)
				);

			foreach (var result in results)
			{
				sequenceSetup.Returns(result);
			}

			return this;
		}
		{{~else if !method.hasResult && !method.isTask~}}

		public {{className}} Setup{{method.fullName}}(
			Func<Exception> result = null{{if method.parameters.size > 0}},{{end}}
			{{-for parameter in method.parameters}}
			Func<{{parameter.type}}, bool> is{{parameter.originalName.pascalCase}} = null{{if !for.last}},{{end}}
			{{-end}})
		{
			var setup = Setup(
					x => x.{{method.fullName}}(
					{{~for parameter in method.parameters~}}
						It.Is<{{parameter.type}}>(fromMock => is{{parameter.originalName.pascalCase}} == null || is{{parameter.originalName.pascalCase}}(fromMock)){{if !for.last}},{{end}}
					{{~end~}}
					)
				);
				
			if(result != null)
			{
				setup.Throws(result());
			}

			setup.Verifiable();

			return this;
		}

		public {{className}} Setup{{method.name}}Sequence{{method.genericArguments}}(
			IEnumerable<Func<Exception>> results{{if method.parameters.size > 0}},{{end}}
			{{-for parameter in method.parameters}}
			Func<{{parameter.type}}, bool> is{{parameter.originalName.pascalCase}} = null{{if !for.last}},{{end}}
			{{-end}})
		{
			var sequenceSetup =
				SetupSequence(
					x => x.{{method.fullName}}(
					{{~for parameter in method.parameters~}}
						It.Is<{{parameter.type}}>(fromMock => is{{parameter.originalName.pascalCase}} == null || is{{parameter.originalName.pascalCase}}(fromMock)){{if !for.last}},{{end}}
					{{~end~}}
					)
				);

			foreach (var result in results)
			{
				if(result == null)
				{
					sequenceSetup.Pass();
					continue;
				}

				sequenceSetup.Throws(result());
			}

			return this;
		}
		{{~end~}}
		{{~end~}}
		{{~for property in properties~}}

		public {{className}} Setup{{property.name}}({{if property.hasGetter}}Func<{{property.returnType}}> result{{end}})
		{
			{{~if property.hasGetter~}}
			SetupGet(x => x.{{property.name}})
				.Returns(result)
				.Verifiable();

			{{~end~}}
			{{~if property.hasSetter~}}
			SetupSet(x => x.{{property.name}} = It.IsAny<{{property.returnType}}>())
				.Verifiable();

			{{~end~}}
			return this;
		}
		{{~end~}}
	}
}