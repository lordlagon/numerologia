using Bunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Numerologia.Web.Pages;
using Numerologia.Web.Services;

namespace Numerologia.UnitTests.Pages;

public class PerfilTests : TestContext
{
    private readonly IPerfilService _serviceMock;
    private readonly PerfilState _perfilState = new();

    public PerfilTests()
    {
        _serviceMock = Substitute.For<IPerfilService>();
        Services.AddSingleton(_serviceMock);
        Services.AddSingleton(_perfilState);
    }

    [Fact]
    public void Render_DeveExibirEmailENomeDoGoogle()
    {
        _serviceMock.ObterAsync().Returns(new PerfilDto("ana@test.com", "Ana Google", null));

        var cut = RenderComponent<Perfil>();

        cut.WaitForAssertion(() =>
        {
            cut.Markup.Should().Contain("ana@test.com");
            cut.Markup.Should().Contain("Ana Google");
        });
    }

    [Fact]
    public void Render_ComNomeExibicaoCadastrado_DevePreencherCampo()
    {
        _serviceMock.ObterAsync().Returns(new PerfilDto("ana@test.com", "Ana Google", "Ana Numeróloga"));

        var cut = RenderComponent<Perfil>();

        cut.WaitForAssertion(() =>
        {
            var input = cut.Find("[data-testid='nomeExibicao']");
            input.GetAttribute("value").Should().Be("Ana Numeróloga");
        });
    }

    [Fact]
    public async Task Submit_ComNomeValido_ChamaAtualizarAsync()
    {
        _serviceMock.ObterAsync().Returns(new PerfilDto("ana@test.com", "Ana Google", null));
        _serviceMock.AtualizarAsync(Arg.Any<SalvarPerfilDto>())
            .Returns(new PerfilDto("ana@test.com", "Ana Google", "Ana Numeróloga"));

        var cut = RenderComponent<Perfil>();
        cut.WaitForAssertion(() => cut.Find("[data-testid='nomeExibicao']"));

        cut.Find("[data-testid='nomeExibicao']").Change("Ana Numeróloga");
        await cut.Find("form").SubmitAsync();

        await _serviceMock.Received(1).AtualizarAsync(
            Arg.Is<SalvarPerfilDto>(d => d.NomeExibicao == "Ana Numeróloga"));
    }

    [Fact]
    public async Task Submit_ComSucesso_ExibeMensagemDeSucesso()
    {
        _serviceMock.ObterAsync().Returns(new PerfilDto("ana@test.com", "Ana Google", null));
        _serviceMock.AtualizarAsync(Arg.Any<SalvarPerfilDto>())
            .Returns(new PerfilDto("ana@test.com", "Ana Google", "Ana Numeróloga"));

        var cut = RenderComponent<Perfil>();
        cut.WaitForAssertion(() => cut.Find("[data-testid='nomeExibicao']"));

        await cut.Find("form").SubmitAsync();

        cut.WaitForAssertion(() =>
            cut.Markup.Should().Contain("Perfil atualizado com sucesso"));
    }

    [Fact]
    public async Task Submit_ComSucesso_AtualizaPerfilState()
    {
        _serviceMock.ObterAsync().Returns(new PerfilDto("ana@test.com", "Ana Google", null));
        _serviceMock.AtualizarAsync(Arg.Any<SalvarPerfilDto>())
            .Returns(new PerfilDto("ana@test.com", "Ana Google", "Ana Numeróloga"));

        var cut = RenderComponent<Perfil>();
        cut.WaitForAssertion(() => cut.Find("[data-testid='nomeExibicao']"));

        cut.Find("[data-testid='nomeExibicao']").Change("Ana Numeróloga");
        await cut.Find("form").SubmitAsync();

        _perfilState.NomeExibicao.Should().Be("Ana Numeróloga");
    }

    [Fact]
    public async Task Submit_ComErroDeServico_ExibeMensagemDeErro()
    {
        _serviceMock.ObterAsync().Returns(new PerfilDto("ana@test.com", "Ana Google", null));
        _serviceMock.AtualizarAsync(Arg.Any<SalvarPerfilDto>())
            .Returns<PerfilDto>(_ => throw new HttpRequestException("Erro"));

        var cut = RenderComponent<Perfil>();
        cut.WaitForAssertion(() => cut.Find("[data-testid='nomeExibicao']"));

        await cut.Find("form").SubmitAsync();

        cut.WaitForAssertion(() =>
            cut.Markup.Should().Contain("Erro ao salvar"));
    }
}
