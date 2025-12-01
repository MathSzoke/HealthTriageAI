import React, { useEffect, useState } from 'react'
import {
    Button,
    Input,
    Field,
    Textarea,
    makeStyles,
    shorthands,
    Divider,
    Text,
} from '@fluentui/react-components'
import { ArrowClockwise24Regular } from '@fluentui/react-icons'

const useStyles = makeStyles({
    form: { display: 'grid', ...shorthands.gap('12px'), backgroundColor: 'var(--colorNeutralBackground1)', ...shorthands.padding('16px'), borderRadius: '12px' },
    row: { display: 'grid', gridTemplateColumns: '1fr auto', alignItems: 'end', columnGap: '8px' },
    actions: { display: 'grid', gridAutoFlow: 'column', justifyContent: 'start', ...shorthands.gap('8px') }
})

function rand(arr) { return Math.floor(Math.random() * arr.length) >= 0 ? arr[Math.floor(Math.random() * arr.length)] : arr[0] }
function randInt(min, max) { return Math.floor(Math.random() * (max - min + 1)) + min }
function randFloat(min, max, dec = 1) { const v = Math.random() * (max - min) + min; return Number(v.toFixed(dec)) }

function genName() {
    const first = ['Ana', 'Bruno', 'Carla', 'Diego', 'Eduarda', 'Fernanda', 'Gustavo', 'Helena', 'Igor', 'Júlia', 'Lucas', 'Marina', 'Nina', 'Otávio', 'Paula', 'Rafa', 'Sofia', 'Thiago', 'Ursula', 'Vitória']
    const last = ['Silva', 'Santos', 'Oliveira', 'Souza', 'Pereira', 'Costa', 'Rodrigues', 'Almeida', 'Nascimento', 'Lima', 'Araújo', 'Gomes']
    return `${rand(first)} ${rand(last)}`
}
function genSymptoms() {
    const base = [
        'febre e dor de cabeça',
        'dor no peito e falta de ar',
        'tosse seca e cansaço',
        'náusea e tontura',
        'dor abdominal e febre',
        'desmaio breve e palpitações',
        'dor lombar e mal-estar',
        'garganta inflamada e febre baixa'
    ]
    return rand(base)
}
function genLocation() {
    const cities = ['São Paulo', 'Rio de Janeiro', 'Belo Horizonte', 'Curitiba', 'Porto Alegre', 'Recife', 'Salvador', 'Fortaleza', 'Florianópolis', 'Campinas']
    return `${rand(cities)}`
}

export default function CaseForm({ onSubmit, connStatus, countLimit, isSubmitting }) {
    const s = useStyles()
    const [form, setForm] = useState({ name: '', age: 30, symptoms: '', temperature: '', heartRate: '', systolicBP: '', location: '' })

    useEffect(() => {
        randomAll()
    }, [])

    function set(k, v) { setForm(prev => ({ ...prev, [k]: v })) }

    function randomName() { set('name', genName()) }
    function randomAge() { set('age', String(randInt(18, 85))) }
    function randomSymptoms() { set('symptoms', genSymptoms()) }
    function randomTemp() { set('temperature', String(randFloat(36.0, 40.5, 1))) }
    function randomHr() { set('heartRate', String(randInt(60, 140))) }
    function randomSbp() { set('systolicBP', String(randInt(80, 160))) }
    function randomLocation() { set('location', genLocation()) }

    function randomAll() {
        setForm({
            name: genName(),
            age: String(randInt(18, 85)),
            symptoms: genSymptoms(),
            temperature: String(randFloat(36.0, 40.5, 1)),
            heartRate: String(randInt(60, 140)),
            systolicBP: String(randInt(80, 160)),
            location: genLocation()
        })
    }

    function submit(e) { e.preventDefault(); onSubmit(form) }

    const isDisabled = countLimit >= 3 || connStatus !== 'connected' || isSubmitting

    return (
        <form className={s.form} onSubmit={submit}>
            <Text weight="semibold">New Triage</Text>

            <div className={s.row}>
                <Field label="Name" required><Input value={form.name} onChange={(_, d) => set('name', d.value)} /></Field>
                <Button type="button" icon={<ArrowClockwise24Regular />} onClick={randomName} />
            </div>

            <div className={s.row}>
                <Field label="Age" required><Input type="number" value={form.age} onChange={(_, d) => set('age', d.value)} /></Field>
                <Button type="button" icon={<ArrowClockwise24Regular />} onClick={randomAge} />
            </div>

            <div className={s.row}>
                <Field label="Symptoms" required><Textarea value={form.symptoms} onChange={(_, d) => set('symptoms', d.value)} /></Field>
                <Button type="button" icon={<ArrowClockwise24Regular />} onClick={randomSymptoms} />
            </div>

            <Divider />

            <div className={s.row}>
                <Field label="Temperature (°C) - (optional)"><Input type="number" step="0.1" value={form.temperature} onChange={(_, d) => set('temperature', d.value)} /></Field>
                <Button type="button" icon={<ArrowClockwise24Regular />} onClick={randomTemp} />
            </div>

            <div className={s.row}>
                <Field label="Heart Rate (bpm) - (optional)"><Input type="number" value={form.heartRate} onChange={(_, d) => set('heartRate', d.value)} /></Field>
                <Button type="button" icon={<ArrowClockwise24Regular />} onClick={randomHr} />
            </div>

            <div className={s.row}>
                <Field label="Systolic BP (mmHg) - (optional)"><Input type="number" value={form.systolicBP} onChange={(_, d) => set('systolicBP', d.value)} /></Field>
                <Button type="button" icon={<ArrowClockwise24Regular />} onClick={randomSbp} />
            </div>

            <div className={s.row}>
                <Field label="Location (optional)"><Input value={form.location} onChange={(_, d) => set('location', d.value)} /></Field>
                <Button type="button" icon={<ArrowClockwise24Regular />} onClick={randomLocation} />
            </div>

            <div className={s.actions}>
                <Button icon={<ArrowClockwise24Regular />} appearance="secondary" onClick={randomAll} type="button">Randomize All</Button>
                <Button
                    appearance="primary"
                    type="submit"
                    disabled={isDisabled}
                >
                    {isSubmitting ? 'Sending...' : 'Send'}
                </Button>
            </div>
        </form>
    )
}
