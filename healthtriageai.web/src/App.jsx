import React, { useEffect, useMemo, useState } from 'react'
import * as signalR from '@microsoft/signalr'
import {
    makeStyles,
    shorthands,
    Spinner,
    Title3,
    Divider,
    Toolbar,
    ToolbarButton,
    ToolbarDivider,
    TabList,
    Tab,
    Input,
    Tooltip,
    Popover,
    PopoverTrigger,
    PopoverSurface,
    MessageBar,
    MessageBarTitle,
    MessageBarBody
} from '@fluentui/react-components'
import {
    Dismiss24Regular,
    Pulse24Regular,
    Search24Regular,
    Navigation24Regular
} from '@fluentui/react-icons'
import CaseForm from './Components/Case/Form/CaseForm.jsx'
import CaseCard from './Components/Case/Card/CaseCard.jsx'
import { levelToKey } from './utils/enums.js'

const STORAGE_CASES_KEY = 'triage_cases'
const STORAGE_DATE_KEY = 'triage_cases_date'

const useStyles = makeStyles({
    root: {
        display: 'grid',
        minHeight: '100vh',
        height: '100%',
        backgroundColor: 'var(--colorNeutralBackground2)'
    },
    topbar: {
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'space-between',
        padding: '12px 16px'
    },
    toolbar: {
        display: 'grid',
        gridAutoFlow: 'column',
        alignItems: 'center',
        ...shorthands.gap('8px')
    },
    filterbar: {
        display: 'grid',
        height: 'max-content',
        gridTemplateColumns: 'auto 1fr auto',
        alignItems: 'center',
        padding: '8px 16px',
        backgroundColor: 'var(--colorNeutralBackground1)'
    },
    tabsInline: {
        display: 'block',
        '@media (max-width: 600px)': {
            display: 'none'
        }
    },
    tabsMobileTrigger: {
        display: 'none',
        '@media (max-width: 600px)': {
            display: 'grid'
        }
    },
    searchWrap: {
        display: 'grid',
        gridAutoFlow: 'column',
        alignItems: 'center',
        ...shorthands.gap('8px')
    },
    content: {
        display: 'grid',
        gridTemplateColumns: '400px 1fr',
        ...shorthands.gap('16px'),
        padding: '16px',
        '@media (max-width: 690px)': {
            gridTemplateColumns: 'auto'
        }
    },
    left: {
        display: 'grid',
        alignContent: 'start',
        ...shorthands.gap('16px')
    },
    right: {
        overflow: 'auto',
        display: 'grid',
        alignContent: 'start',
        ...shorthands.gap('12px')
    },
    list: {
        display: 'grid',
        gridTemplateColumns: 'repeat(auto-fill, minmax(360px, 1fr))',
        ...shorthands.gap('12px')
    }
})

function getTodayKey() {
    const d = new Date()
    const y = d.getFullYear()
    const m = String(d.getMonth() + 1).padStart(2, '0')
    const day = String(d.getDate()).padStart(2, '0')
    return `${y}-${m}-${day}`
}

export default function App() {
    const s = useStyles()
    const [connStatus, setConnStatus] = useState('connecting')
    const [filter, setFilter] = useState('all')
    const [q, setQ] = useState('')
    const [isMobile, setIsMobile] = useState(window.matchMedia('(max-width: 600px)').matches)
    const [tabsOpen, setTabsOpen] = useState(false)
    const [isSubmitting, setIsSubmitting] = useState(false)
    const [cases, setCases] = useState(() => {
        const stored = localStorage.getItem(STORAGE_CASES_KEY)
        if (!stored) return []
        try {
            const parsed = JSON.parse(stored)
            return Array.isArray(parsed) ? parsed : []
        } catch {
            return []
        }
    })
    const api = import.meta.env.VITE_API_BASE
    const map = useMemo(() => new Map(), [])

    useEffect(() => {
        const todayKey = getTodayKey()
        const storedDate = localStorage.getItem(STORAGE_DATE_KEY)
        if (storedDate !== todayKey) {
            localStorage.setItem(STORAGE_DATE_KEY, todayKey)
            localStorage.removeItem(STORAGE_CASES_KEY)
            setCases([])
        }
    }, [])

    useEffect(() => {
        const media = window.matchMedia('(max-width: 600px)')
        const handler = e => setIsMobile(e.matches)
        media.addEventListener('change', handler)
        return () => media.removeEventListener('change', handler)
    }, [])

    useEffect(() => {
        cases.forEach(c => {
            if (c && c.id) {
                map.set(c.id, c)
            }
        })
        if (cases.length > 0) {
            setCases(toSortedList(map))
        }
    }, [])

    useEffect(() => {
        localStorage.setItem(STORAGE_CASES_KEY, JSON.stringify(cases))
    }, [cases])

    useEffect(() => {
        const c = new signalR.HubConnectionBuilder()
            .withUrl(`${api}/hubs/triage`)
            .withAutomaticReconnect()
            .build()

        c.on('case_updated', payload => {
            map.set(payload.id, payload)
            setCases(toSortedList(map))
        })

        c.on('case_step', step => {
            const item = map.get(step.id)
            if (!item) return
            const steps = item.steps ? [...item.steps, step] : [step]
            map.set(step.id, { ...item, steps })
            setCases(toSortedList(map))
        })

        c.start()
            .then(() => setConnStatus('connected'))
            .catch(() => setConnStatus('error'))

        c.onreconnected(() => setConnStatus('connected'))
        c.onreconnecting(() => setConnStatus('connecting'))
        c.onclose(() => setConnStatus('closed'))

        return () => {
            c.stop()
        }
    }, [api])

    function toSortedList(m) {
        return Array.from(m.values()).sort(
            (a, b) => new Date(b.createdAt) - new Date(a.createdAt)
        )
    }

    async function submitCase(form) {
        if (cases.length >= 3) {
            return
        }
        if (isSubmitting) {
            return
        }

        setIsSubmitting(true)

        const body = {
            name: form.name,
            age: Number(form.age),
            symptoms: form.symptoms,
            temperature: form.temperature ? Number(form.temperature) : null,
            heartRate: form.heartRate ? Number(form.heartRate) : null,
            systolicBP: form.systolicBP ? Number(form.systolicBP) : null,
            location: form.location
        }

        try {
            const result = await fetch(`${api}/api/triage/report`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(body)
            })
        } catch {
        } finally {
            setIsSubmitting(false)
        }
    }

    const filtered = cases
        .filter(c => (filter === 'all' ? true : levelToKey(c.level) === filter))
        .filter(c =>
            !q
                ? true
                : `${c.name} ${c.symptoms} ${c.location}`
                    .toLowerCase()
                    .includes(q.toLowerCase())
        )

    return (
        <div className={s.root}>
            <div className={s.topbar}>
                <Title3>Health Triage</Title3>
                <Toolbar className={s.toolbar}>
                    <Tooltip content={connStatus} relationship="label">
                        <ToolbarButton
                            icon={<Pulse24Regular />}
                            appearance={connStatus === 'connected' ? 'primary' : 'subtle'}
                        />
                    </Tooltip>
                    <ToolbarDivider />
                    <ToolbarButton
                        icon={<Dismiss24Regular />}
                        onClick={() => setCases([])}
                    >
                        Clear
                    </ToolbarButton>
                </Toolbar>
            </div>

            <div className={s.filterbar}>
                <div className={s.tabsInline}>
                    <TabList
                        selectedValue={filter}
                        onTabSelect={(_, d) => setFilter(String(d.value))}
                        appearance="subtle"
                    >
                        <Tab value="all">All</Tab>
                        <Tab value="low">Low</Tab>
                        <Tab value="moderate">Moderate</Tab>
                        <Tab value="high">High</Tab>
                        <Tab value="critical">Critical</Tab>
                    </TabList>
                </div>

                <div />

                <div className={s.tabsMobileTrigger}>
                    <Popover
                        open={tabsOpen}
                        onOpenChange={(_, data) => setTabsOpen(!!data.open)}
                        withArrow
                    >
                        <PopoverTrigger disableButtonEnhancement>
                            <ToolbarButton
                                icon={<Navigation24Regular />}
                                appearance="subtle"
                                aria-label="Filter"
                            />
                        </PopoverTrigger>
                        <PopoverSurface>
                            <TabList
                                selectedValue={filter}
                                onTabSelect={(_, d) => {
                                    setFilter(String(d.value))
                                    setTabsOpen(false)
                                }}
                                appearance="subtle"
                                vertical
                            >
                                <Tab value="all">All</Tab>
                                <Tab value="low">Low</Tab>
                                <Tab value="moderate">Moderate</Tab>
                                <Tab value="high">High</Tab>
                                <Tab value="critical">Critical</Tab>
                            </TabList>
                        </PopoverSurface>
                    </Popover>
                </div>

                <div className={s.searchWrap}>
                    <Search24Regular />
                    <Input
                        placeholder="Search by name, symptoms or location"
                        value={q}
                        onChange={(_, d) => setQ(d.value)}
                    />
                </div>
            </div>

            <Divider />

            <div className={s.content}>
                <div className={s.left}>
                    <CaseForm
                        onSubmit={submitCase}
                        connStatus={connStatus}
                        countLimit={cases.length}
                        isSubmitting={isSubmitting}
                    />
                    {cases.length >= 3 &&
                        <MessageBar style={{ width: '400px' }} intent={"warning"}>
                            <MessageBarBody>
                                <MessageBarTitle>Limit reached</MessageBarTitle>
                                I'm not rich yet, please wait until tomorrow to try again.
                            </MessageBarBody>
                        </MessageBar>
                    }
                </div>
                <div className={s.right}>
                    {connStatus === 'connecting' && <Spinner label="Connecting..." />}
                    <div className={s.list}>
                        {filtered.map(c => (
                            <CaseCard key={c.id} data={c} />
                        ))}
                    </div>
                </div>
            </div>
        </div>
    )
}
