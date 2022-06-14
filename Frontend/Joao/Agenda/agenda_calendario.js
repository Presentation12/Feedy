let calendario = document.querySelector('.calendar')

const meses_nomes = ['Janeiro', 'Fevereiro', 'Março', 'Abril', 'Maio', 'Junho', 'Julho', 'Agosto', 'Setembro', 'Outubro', 'Novembro', 'Dezembror']

verifica_bissexto = (ano) => {
    return (ano % 4 === 0 && ano % 100 !== 0 && ano % 400 !== 0) || (ano % 100 === 0 && ano % 400 ===0)
}

dias_fevereiro = (ano) => {
    return verifica_bissexto(ano) ? 29 : 28
}

gerar_calendario = (mes, ano) => {

    let dias_calendario = calendario.querySelector('.calendar-days')
    let ano_cabecalho = calendario.querySelector('#year')

    let dias_mes = [31, dias_fevereiro(ano), 31, 30, 31, 30, 31, 31, 30, 31, 30, 31]

    dias_calendario.innerHTML = ''

    let data_atual = new Date()
    if (!mes) mes = data_atual.getMonth()
    if (!ano) ano = data_atual.getFullYear()

    let mes_atual = `${meses_nomes[mes]}`
    mes_escolhido.innerHTML = mes_atual
    ano_cabecalho.innerHTML = ano

    // get first day of mes
    
    let primeiro_dia = new Date(ano, mes, 1)

    for (let i = 0; i <= dias_mes[mes] + primeiro_dia.getDay() - 1; i++) {
        let dia = document.createElement('div')
        if (i >= primeiro_dia.getDay()) {
            dia.classList.add('calendar-day-hover')
            dia.innerHTML = i - primeiro_dia.getDay() + 1
            dia.innerHTML += `<span></span>
                            <span></span>
                            <span></span>
                            <span></span>`
            if (i - primeiro_dia.getDay() + 1 === data_atual.getDate() && ano === data_atual.getFullYear() && mes === data_atual.getMonth()) {
                dia.classList.add('curr-date')
            }
        }

        dia.onclick = () => {
            dia.classList.add('select-date')
            for (let i = 0; i <= dias_mes[mes] + primeiro_dia.getDay() - 1; i++)
            {
                i.classList.remove('select-date')
            }
        }

        dias_calendario.appendChild(dia)
    }
}

let lista_meses = calendario.querySelector('.month-list')

meses_nomes.forEach((e, index) => {
    let mes = document.createElement('div')
    mes.innerHTML = `<div data-month="${index}">${e}</div>`
    mes.querySelector('div').onclick = () => {
        lista_meses.classList.remove('show')
        mes_atual.value = index
        gerar_calendario(index, ano_atual.value)
    }
    lista_meses.appendChild(mes)
})

let mes_escolhido = calendario.querySelector('#month-picker')

mes_escolhido.onclick = () => {
    lista_meses.classList.add('show')
}


let data_atual = new Date()

let mes_atual = {value: data_atual.getMonth()}
let ano_atual = {value: data_atual.getFullYear()}

gerar_calendario(mes_atual.value, ano_atual.value)

document.querySelector('#prev-year').onclick = () => {
    --ano_atual.value
    gerar_calendario(mes_atual.value, ano_atual.value)
}

document.querySelector('#next-year').onclick = () => {
    ++ano_atual.value
    gerar_calendario(mes_atual.value, ano_atual.value)
}