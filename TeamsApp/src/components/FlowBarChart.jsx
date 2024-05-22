import React from 'react';
import { BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer, LabelList } from 'recharts';

const FlowBarChart = ({ flows }) => {
    if (!flows || flows.length === 0) {
        return <div>No data available</div>;
    }

    const barColors = [
        "#1A1246",
        "#6C225B",
        "#B1405F",
        "#E47357",
        "#FDB354",
        "#F9F871",
    ];

    // Restructure data to have a bar for each flowId
    const transformedData = flows.map((flow, index) => ({
        flowId: flow.name, // assuming name is the flowId
        ...flow.data.reduce((acc, curr) => {
            acc[`runCount${index}`] = curr.runCount;
            return acc;
        }, {})
    }));

    // Extract all unique flowIds
    const flowIds = flows.map(flow => flow.name);

    // Custom label formatter function
    const labelFormatter = (value, entry) => {
        if (value === 50) {
            return `${value}+`;
        }
        return value;
    };

    return (
        <ResponsiveContainer width="100%" height="100%" maxHeight={500}>
            <BarChart
                data={transformedData}
                margin={{ top: 20, right: 30, left: 20, bottom: 20 }}
            >
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis
                    dataKey="flowId"
                    tickFormatter={(tick) => tick}
                />
                <YAxis />
                <Tooltip />
                {flowIds.map((flowId, index) => (
                    <Bar
                        key={flowId}
                        dataKey={`runCount${index}`}
                        fill={barColors[index % barColors.length]}
                        name={flowId}
                        barSize={30}
                        stackId="a"
                    >
                        <LabelList dataKey={`runCount${index}`} position="top" formatter={labelFormatter} />
                    </Bar>
                ))}
            </BarChart>
        </ResponsiveContainer>
    );
};

export default FlowBarChart;